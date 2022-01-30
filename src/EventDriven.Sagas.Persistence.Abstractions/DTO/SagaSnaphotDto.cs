using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Persistence.Abstractions.DTO;

/// <summary>
/// Saga snapshot.
/// </summary>
public class SagaSnapshotDto
{
    /// <summary>
    /// Saga snapshot identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Time the snapshot was created.
    /// </summary>
    public DateTime SnapshotCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Saga identifier.
    /// </summary>
    public Guid SagaId { get; set; }

    /// <summary>
    /// Time the saga was started.
    /// </summary>
    public DateTime SagaStarted { get; set; }

    /// <summary>
    /// Saga snapshot sequence.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Entity identifier.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// The current saga step.
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// State of the saga.
    /// </summary>
    public SagaState State { get; set; }

    /// <summary>
    /// Information about the state of the saga.
    /// </summary>
    public string? StateInfo { get; set; }

    /// <summary>
    /// Represents a unique ID that must change atomically with each store of the entity
    /// to its underlying storage medium.
    /// </summary>
    public string ETag { get; set; } = Guid.Empty.ToString();

    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid? SagaConfigId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga configuration name.
    /// </summary>
    public string? SagaConfigName { get; set; }

    /// <summary>
    /// Steps performed by the saga.
    /// </summary>
    public List<SagaStepDto?> Steps { get; set; } = null!;
}