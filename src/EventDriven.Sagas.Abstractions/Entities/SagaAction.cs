using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Entities;

/// <summary>
/// An action performed in a saga step.
/// </summary>
public record SagaAction
{
    /// <summary>
    /// Saga action identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga action command.
    /// </summary>
    public SagaCommand Command { get; set; } = null!;

    /// <summary>
    /// Saga action timeout.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// State of the action.
    /// </summary>
    public ActionState State { get; set; }

    /// <summary>
    /// Information about the state of the action.
    /// </summary>
    public string? StateInfo { get; set; }

    /// <summary>
    /// When the action started.
    /// </summary>
    public DateTime? Started { get; set; }

    /// <summary>
    /// When the action completed.
    /// </summary>
    public DateTime? Completed { get; set; }

    /// <summary>
    /// Saga action duration.
    /// </summary>
    public TimeSpan? Duration { get; set; }
}