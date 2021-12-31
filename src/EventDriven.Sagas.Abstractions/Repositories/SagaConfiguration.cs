namespace EventDriven.Sagas.Abstractions.Repositories;

/// <summary>
/// Saga configuration.
/// </summary>
public class SagaConfiguration
{
    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Optional saga configuration name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public Dictionary<int, SagaStep> Steps { get; set; } = null!;
}