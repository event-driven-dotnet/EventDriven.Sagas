namespace EventDriven.Sagas.Configuration.Abstractions.DTO;

/// <summary>
/// An action performed in a saga step.
/// </summary>
public class SagaActionDto
{
    /// <summary>
    /// Saga action identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga action command.
    /// </summary>
    public string Command { get; set; } = null!;

    /// <summary>
    /// Saga action timeout.
    /// </summary>
    public TimeSpan? Timeout { get; set; }
    
    /// <summary>
    /// Set to true to reverse action on failure.
    /// </summary>
    public bool ReverseOnFailure { get; set; }
}