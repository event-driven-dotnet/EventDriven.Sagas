using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Configuration.Abstractions;

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
    /// Represents a unique ID that must change atomically with each store of the entity
    /// to its underlying storage medium.
    /// </summary>
    public string ETag { get; set; } = Guid.Empty.ToString();

    /// <summary>
    /// Optional saga configuration name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public List<SagaStep> Steps { get; set; } = null!;
}