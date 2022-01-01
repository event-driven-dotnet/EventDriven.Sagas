namespace EventDriven.Sagas.Abstractions.Commands;

/// <inheritdoc />
public abstract class CommandResultEvaluator<TResult, TExpectedResult>
    : ICommandResultEvaluator<TResult, TExpectedResult>
{
    private string _timeoutMessage = "Duration exceeded timeout.";
    private string _failureMessage = "Unexpected result.";

    /// <summary>
    /// Command result.
    /// </summary>
    protected TResult? Result { get; set; }

    /// <summary>
    /// Command expected result.
    /// </summary>
    protected TExpectedResult? ExpectedResult { get; set; }

    /// <summary>
    /// Command duration.
    /// </summary>
    protected TimeSpan? Duration { get; set; }

    /// <summary>
    /// Command timeout.
    /// </summary>
    protected TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Cancellation message.
    /// </summary>
    protected virtual string CancellationMessage { get; set; } = "Cancellation requested.";

    /// <summary>
    /// Timeout message.
    /// </summary>
    protected virtual string TimeoutMessage
    {
        get
        {
            if (Timeout == null || Duration == null) return _timeoutMessage;
            return $"Duration of '{Duration!.Value:c}' exceeded timeout of '{Timeout!.Value:c}'";
        }
        set => _timeoutMessage = value;
    }

    /// <summary>
    /// Failure message.
    /// </summary>
    protected virtual string FailureMessage
    {
        get
        {
            if (Result == null || ExpectedResult == null) return _failureMessage;
            return $"'{Result}' returned when '{ExpectedResult}' was expected.";
        }
        set => _failureMessage = value;
    }

    /// <summary>
    /// Saga state information.
    /// </summary>
    public virtual string? SagaStateInfo { get; set; }

    /// <inheritdoc />
    public abstract Task<bool> EvaluateCommandResultAsync(TResult commandResult, TExpectedResult expectedResult);

    /// <inheritdoc />
    public virtual async Task<bool> EvaluateStepResultAsync(
        SagaStep step, bool compensating, CancellationToken cancellationToken)
    {
        // Evaluate result
        var isCancelled = !compensating && cancellationToken.IsCancellationRequested;
        var action = compensating ? step.CompensatingAction : step.Action;
        if (action.Command is not ISagaCommand<TResult, TExpectedResult> command) return false;
        Result = command.Result;
        ExpectedResult = command.ExpectedResult;
        var commandSuccessful = !isCancelled
            && await EvaluateCommandResultAsync(command.Result, command.ExpectedResult);

        // Check timeout
        action.Completed = DateTime.UtcNow;
        action.Duration = action.Completed - action.Started;
        Duration = action.Duration;
        Timeout = action.Timeout;
        var commandTimedOut = commandSuccessful && action.Timeout != null && action.Duration > action.Timeout;
        if (commandTimedOut) commandSuccessful = false;

        // Transition action state
        action.State = ActionState.Succeeded;
        if (!commandSuccessful)
        {
            if (isCancelled)
            {
                action.State = ActionState.Cancelled;
                action.StateInfo = CancellationMessage;
            }
            else if (!commandTimedOut)
            {
                action.State = ActionState.Failed;
                action.StateInfo = FailureMessage;
            }
            else
            {
                action.State = ActionState.TimedOut;
                action.StateInfo = TimeoutMessage;
            }

            var commandName = action.Command.Name ?? "No name";
            SagaStateInfo = $"Step {step.Sequence} command '{commandName}' failed. {action.StateInfo}";
            return false;
        }

        return true;
    }
}