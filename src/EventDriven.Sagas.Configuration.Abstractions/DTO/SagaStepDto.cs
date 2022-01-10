namespace EventDriven.Sagas.Configuration.Abstractions.DTO;

/// <summary>
/// Saga step consisting of both an action and compensating action.
/// </summary>
public class SagaStepDto
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
    public SagaActionDto Action { get; set; } = null!;

    /// <summary>
    /// Saga step compensating action.
    /// </summary>
    public SagaActionDto CompensatingAction { get; set; } = null!;
}