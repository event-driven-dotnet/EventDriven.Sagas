using EventDriven.Sagas.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions.Entities;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract record Saga
{
    /// <summary>
    /// Cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken;

    /// <summary>
    /// Saga identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Represents a unique ID that must change atomically with each store of the entity
    /// to its underlying storage medium.
    /// </summary>
    public string ETag { get; set; } = null!;

    /// <summary>
    /// State of the saga.
    /// </summary>
    public SagaState State { get; set; }

    /// <summary>
    /// Information about the state of the saga.
    /// </summary>
    public string? StateInfo { get; set; }

    /// <summary>
    /// The current saga step.
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public List<SagaStep> Steps { get; set; } = new();

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public ISagaCommandDispatcher? SagaCommandDispatcher { get; set; }

    /// <summary>
    /// Command result evaluator.
    /// </summary>
    public ICommandResultEvaluator? CommandResultEvaluator { get; set; }

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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set state, current step, cancellation token
        State = SagaState.Executing;
        CurrentStep = 1;
        CancellationToken = cancellationToken;

        // Dispatch current step command
        await ExecuteCurrentActionAsync();
    }
}

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public abstract record Saga<TEntity> : Saga
{
    /// <summary>
    /// SagaConfig constructor.
    /// Note: To use <see cref="IServiceCollection"/>.AddSaga, inheritors must have a parameterless constructor. 
    /// </summary>
    // ReSharper disable once EmptyConstructor
    protected Saga()
    {
    }

    /// <summary>
    /// Entity.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public TEntity Entity { get; set; } = default!;

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="entity">Saga entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Entity = entity;
        await StartSagaAsync(cancellationToken);
    }
}
