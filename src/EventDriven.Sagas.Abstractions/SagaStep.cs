namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Saga step consisting of both an action and compensating action.
/// </summary>
public class SagaStep
{
    /// <summary>
    /// Saga step identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga step sequence.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Saga step action.
    /// </summary>
    public SagaAction Action { get; set; } = null!;

    /// <summary>
    /// Saga step compensating action.
    /// </summary>
    public SagaAction CompensatingAction { get; set; } = null!;
}