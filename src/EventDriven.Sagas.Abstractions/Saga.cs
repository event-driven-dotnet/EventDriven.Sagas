﻿using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class Saga
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandDispatcher">Saga command dispatcher.</param>
    /// <param name="commandResultEvaluators">Command result evaluators.</param>
    protected Saga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        CommandResultEvaluators = commandResultEvaluators;
    }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken;

    /// <summary>
    /// Saga identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Time the saga was started.
    /// </summary>
    public DateTime Started { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Saga snapshot sequence.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Entity identifier.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// The current saga step.
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// State of the saga.
    /// </summary>
    public SagaState State { get; set; }

    /// <summary>
    /// Information about the state of the saga.
    /// </summary>
    public string? StateInfo { get; set; }

    /// <summary>
    /// Represents a unique ID that must change atomically with each store of the entity
    /// to its underlying storage medium.
    /// </summary>
    public string ETag { get; set; } = null!;

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public List<SagaStep> Steps { get; set; } = new();

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    protected ISagaCommandDispatcher SagaCommandDispatcher { get; set; }

    /// <summary>
    /// Command result evaluators.
    /// </summary>
    protected IEnumerable<ISagaCommandResultEvaluator> CommandResultEvaluators { get; set; }

    /// <summary>
    /// Execute the current action.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task ExecuteCurrentActionAsync();

    /// <summary>
    /// Execute the current compensating action.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task ExecuteCurrentCompensatingActionAsync();

    /// <summary>
    /// Get command result evaluator for this saga by result type.
    /// </summary>
    /// <typeparam name="TSaga">Saga type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <typeparam name="TExpectedResult">Expected result type.</typeparam>
    /// <returns>ISagaCommandResultEvaluator for this saga with the specified result type.</returns>
    protected virtual ISagaCommandResultEvaluator<TResult, TExpectedResult>? GetCommandResultEvaluatorByResultType
        <TSaga, TResult, TExpectedResult>()
        where TSaga : Saga =>
        CommandResultEvaluators
            .Where(e => e.SagaType == null || e.SagaType == typeof(TSaga))
            .OfType<ISagaCommandResultEvaluator<TResult, TExpectedResult>>().FirstOrDefault();

    /// <summary>
    /// Handle command result for step.
    /// </summary>
    /// <param name="step">Saga step.</param>
    /// <param name="compensating">True if compensating step.</param>
    /// <typeparam name="TSaga">Saga type.</typeparam>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <typeparam name="TExpectedResult">Expected result type.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task HandleCommandResultForStepAsync<TSaga, TResult, TExpectedResult>(SagaStep step, bool compensating)
        where TSaga : Saga
    {
        var evaluator = GetCommandResultEvaluatorByResultType<TSaga, TResult, TExpectedResult>();
        var commandSuccessful = evaluator != null
            && await EvaluateStepResultAsync(step, compensating, evaluator, CancellationToken);
        await TransitionSagaStateAsync(commandSuccessful);
    }
    
    /// <summary>
    /// Evaluate a step result.
    /// </summary>
    /// <param name="step">Saga step.</param>
    /// <param name="compensating">True if compensating step.</param>
    /// <param name="evaluator">Saga command result evaluator.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <typeparam name="TExpectedResult">Expected result type.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a boolean that is true if step completed successfully.
    /// </returns>
    public virtual async Task<bool> EvaluateStepResultAsync<TResult, TExpectedResult>(
        SagaStep step, bool compensating, ISagaCommandResultEvaluator<TResult, TExpectedResult> evaluator,
        CancellationToken cancellationToken)
    {
        // Evaluate result
        var isCancelled = !compensating && cancellationToken.IsCancellationRequested;
        var action = compensating ? step.CompensatingAction : step.Action;
        if (action.Command is not ISagaCommand<TResult, TExpectedResult> command) return false;
        var result = command.Result;
        var expectedResult = command.ExpectedResult;
        var commandSuccessful = !isCancelled
            && await evaluator.EvaluateCommandResultAsync(command.Result, command.ExpectedResult);

        // Check timeout
        action.Completed = DateTime.UtcNow;
        action.Duration = action.Completed - action.Started;
        var duration = action.Duration;
        var timeout = action.Timeout;
        var commandTimedOut = commandSuccessful && action.Timeout != null && action.Duration > action.Timeout;
        if (commandTimedOut) commandSuccessful = false;

        // Transition action state
        action.State = ActionState.Succeeded;
        if (!commandSuccessful)
        {
            if (isCancelled)
            {
                action.State = ActionState.Cancelled;
                action.StateInfo = GetCancellationMessage();
            }
            else if (!commandTimedOut)
            {
                action.State = ActionState.Failed;
                action.StateInfo = GetFailureMessage(result, expectedResult);
            }
            else
            {
                action.State = ActionState.TimedOut;
                action.StateInfo = GetTimeoutMessage(timeout, duration);
            }

            var commandName = action.Command.Name ?? "No name";
            StateInfo = $"Step {step.Sequence} command '{commandName}' failed. {action.StateInfo}";
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Get cancellation message.
    /// </summary>
    /// <returns>The cancellation message.</returns>
    protected virtual string GetCancellationMessage() => "Cancellation requested.";
    
    /// <summary>
    /// Get failure message.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <typeparam name="TExpectedResult">Expected result type.</typeparam>
    /// <returns>The failure message.</returns>
    protected virtual string GetFailureMessage<TResult, TExpectedResult>(
        TResult result, TExpectedResult expectedResult) =>
        result == null || expectedResult == null
            ? "Unexpected result."
            : $"'{result}' returned when '{expectedResult}' was expected.";
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected virtual string GetTimeoutMessage(
        TimeSpan? timeout ,TimeSpan? duration) =>
        timeout == null || duration == null
            ? "Duration exceeded timeout."
            : $"Duration of '{duration.Value:c}' exceeded timeout of '{timeout.Value:c}'";

    /// <summary>
    /// Transition saga state.
    /// </summary>
    /// <param name="commandSuccessful">True if command was successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task TransitionSagaStateAsync(bool commandSuccessful)
    {
        switch (State)
        {
            case SagaState.Executing:
                if (CurrentStep < Steps.Max(s => s.Sequence))
                {
                    if (commandSuccessful)
                    {
                        CurrentStep++;
                        await ExecuteCurrentActionAsync();
                    }
                    else
                    {
                        State = SagaState.Compensating;
                        await ExecuteCurrentCompensatingActionAsync();
                    }
                }
                else
                {
                    State = commandSuccessful ? SagaState.Executed : SagaState.Compensating;
                    if (!commandSuccessful)
                        await ExecuteCurrentCompensatingActionAsync();
                }
                return;
            case SagaState.Compensating:
                if (!commandSuccessful) // Exit if compensating action unsuccessful
                    return;
                if (CurrentStep > Steps.Min(s => s.Sequence))
                {
                    CurrentStep--;
                    await ExecuteCurrentCompensatingActionAsync();
                }
                else
                {
                    State = SagaState.Compensated;
                }
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="entityId">Entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(Guid entityId = default, CancellationToken cancellationToken = default)
    {
        // Set state, current step, entity id, cancellation token
        State = SagaState.Executing;
        CurrentStep = 1;
        EntityId = entityId;
        CancellationToken = cancellationToken;

        // Dispatch current step command
        await ExecuteCurrentActionAsync();
    }
}