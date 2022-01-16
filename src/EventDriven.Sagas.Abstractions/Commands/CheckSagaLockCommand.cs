namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Check saga lock command.
/// </summary>
public record CheckSagaLockCommand : SagaCommand
{
    /// <inheritdoc />
    public CheckSagaLockCommand(Guid entityId)
    {
        EntityId = entityId;
        Name = "Check Saga Lock";
    }
}