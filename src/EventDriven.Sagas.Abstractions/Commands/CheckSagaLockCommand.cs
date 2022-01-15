namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Check saga lock command.
/// </summary>
/// <param name="EntityId">Entity identifier.</param>
public record CheckSagaLockCommand(Guid EntityId) : ISagaCommand
{
    /// <inheritdoc />
    public string? Name { get; set; } = "Check Saga Lock";
}