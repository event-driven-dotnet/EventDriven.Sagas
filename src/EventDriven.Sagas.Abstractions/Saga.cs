namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class Saga
{
    /// <summary>
    /// Saga identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

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
    public Dictionary<int, SagaStep> Steps { get; set; } = new();

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the asynchronous operation.</returns>
    public abstract Task StartSagaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute the current action.
    /// </summary>
    /// <returns>Result of the asynchronous operation.</returns>
    protected abstract Task ExecuteCurrentActionAsync();

    /// <summary>
    /// Execute the current compensating action.
    /// </summary>
    /// <returns>Result of the asynchronous operation.</returns>
    protected abstract Task ExecuteCurrentCompensatingActionAsync();

    /// <summary>
    /// Transition saga state.
    /// </summary>
    /// <param name="commandSuccessful">True if command was successful.</param>
    /// <returns>Result of the asynchronous operation.</returns>
    protected virtual async Task TransitionSagaStateAsync(bool commandSuccessful)
    {
        switch (State)
        {
            case SagaState.Executing:
                if (CurrentStep < Steps.Keys.Max())
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
                if (CurrentStep > Steps.Keys.Min())
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
}