using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <inheritdoc />
public abstract class SagaCommandDispatcher : ISagaCommandDispatcher
{
    /// <summary>
    /// Saga command handlers.
    /// </summary>
    protected IEnumerable<ISagaCommandHandler>  SagaCommandHandlers { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandHandlers">Saga command handlers.</param>
    protected SagaCommandDispatcher(IEnumerable<ISagaCommandHandler> sagaCommandHandlers)
    {
        SagaCommandHandlers = sagaCommandHandlers;
    }

    /// <inheritdoc />
    public abstract Task DispatchCommandAsync(SagaCommand command, bool compensating);

    /// <summary>
    /// Get saga command handler by command type.
    /// </summary>
    /// <param name="sagaCommand">Saga command.</param>
    /// <typeparam name="TSagaCommand">Saga command type.</typeparam>
    /// <returns>Strongly typed ISagaCommandHandler.</returns>
    protected virtual ISagaCommandHandler<TSagaCommand>? GetSagaCommandHandlerByCommandType<TSagaCommand>(
        SagaCommand sagaCommand)
        where TSagaCommand : class, ISagaCommand =>
        SagaCommandHandlers.OfType<ISagaCommandHandler<TSagaCommand>>().FirstOrDefault();

    /// <summary>
    /// Get saga command handler by command type.
    /// </summary>
    /// <param name="sagaCommand">Saga command.</param>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TSagaCommand">Saga command type.</typeparam>
    /// <returns>Strongly typed ISagaCommandHandler.</returns>
    protected virtual ISagaCommandHandler<TEntity, TSagaCommand>? GetSagaCommandHandlerByCommandType<TEntity, TSagaCommand>(
        SagaCommand sagaCommand)
        where TEntity : Entity
        where TSagaCommand : class, ISagaCommand =>
        SagaCommandHandlers.OfType<ISagaCommandHandler<TEntity, TSagaCommand>>().FirstOrDefault();
}