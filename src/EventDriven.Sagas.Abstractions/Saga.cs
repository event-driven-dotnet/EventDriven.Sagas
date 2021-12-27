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
    public SagaStep CurrentStep { get; set; } = null!;

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public List<SagaStep> Steps { get; set; } = new();

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <returns>Result of the asynchronous operation.</returns>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);
}