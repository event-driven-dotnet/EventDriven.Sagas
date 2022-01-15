using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Handlers;

/// <summary>
/// Check saga lock command handler.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
public abstract class CheckSagaLockCommandHandler<TSaga> :
    ICheckSagaLockCommandHandler<CheckSagaLockCommand>
    where TSaga : Saga
{
    /// <inheritdoc />
    public Type? SagaType { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected CheckSagaLockCommandHandler()
    {
        SagaType = typeof(TSaga);
    }

    /// <inheritdoc />
    public abstract Task<bool> HandleCommandAsync(CheckSagaLockCommand command);
}