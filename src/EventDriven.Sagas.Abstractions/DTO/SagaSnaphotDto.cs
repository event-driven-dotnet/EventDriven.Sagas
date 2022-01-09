using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.DTO;

/// <summary>
/// Saga snapshot.
/// </summary>
public class SagaSnapshotDto
{
    /// <summary>
    /// Saga snapshot identifier.
    /// </summary>
    public Guid SnapshotId { get; set; }

    /// <summary>
    /// Time the snapshot was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Saga identifier.
    /// </summary>
    public Guid SagaId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid SagaConfigId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Represents a unique ID that must change atomically with each store of the entity
    /// to its underlying storage medium.
    /// </summary>
    public string ETag { get; set; } = Guid.Empty.ToString();

    /// <summary>
    /// State of the saga.
    /// </summary>
    public SagaState State { get; set; }

    /// <summary>
    /// Optional saga configuration name.
    /// </summary>
    public string? Name { get; set; }

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
    public List<SagaStepDto> Steps { get; set; } = null!;
}