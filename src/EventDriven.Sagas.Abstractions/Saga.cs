using EventDriven.Sagas.Abstractions.Repositories;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class Saga
{
    /// <summary>
    /// Saga protected constructor.
    /// </summary>
    /// <param name="sagaConfigOptions">Saga configuration options.</param>
    protected Saga(SagaConfigurationOptions sagaConfigOptions)
    {
        SagaConfigOptions = sagaConfigOptions;
    }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken;

    /// <summary>
    /// Optional saga configuration identifier.
    /// </summary>
    protected SagaConfigurationOptions SagaConfigOptions { get; set; }

    /// <summary>
    /// Optional saga configuration repository.
    /// </summary>
    protected ISagaConfigRepository? SagaConfigRepository { get; set; }

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
    public Dictionary<int, SagaStep> Steps { get; set; } = new();

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
    /// Configure saga steps.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task ConfigureSteps()
    {
        if (SagaConfigOptions.SagaConfigId != null && SagaConfigRepository != null)
        {
            var sagaConfig = await SagaConfigRepository
                .GetSagaConfigurationAsync(SagaConfigOptions.SagaConfigId.GetValueOrDefault());
            if (sagaConfig != null) Steps = sagaConfig.Steps;
        }
    }

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

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set steps from config
        await ConfigureSteps();

        // Set state, current step, cancellation token
        State = SagaState.Executing;
        CurrentStep = 1;
        CancellationToken = cancellationToken;

        // Dispatch current step command
        await ExecuteCurrentActionAsync();
    }
}