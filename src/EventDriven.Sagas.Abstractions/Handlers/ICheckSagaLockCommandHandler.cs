using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Handlers;

/// <summary>
/// Check lock saga command handler.
/// </summary>
public interface ICheckSagaLockCommandHandler
{
    /// <summary>
    /// Saga type.
    /// </summary>
    public Type? SagaType { get; set; }
}

/// <summary>
/// Check saga lock command handler.
/// </summary>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
public interface ICheckSagaLockCommandHandler<in TSagaCommand> :
    ICheckSagaLockCommandHandler
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The command result.</returns>
    Task<bool> HandleCommandAsync(TSagaCommand command);
}