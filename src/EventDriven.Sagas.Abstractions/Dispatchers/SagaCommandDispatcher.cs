using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Dispatchers;

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
    /// Dispatch command to saga command handler.
    /// </summary>
    /// <param name="command">Saga command.</param>
    /// <typeparam name="TSagaCommand">Saga command type.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a boolean that is true if command dispatched successfully.
    /// </returns>
    protected virtual async Task<bool> DispatchSagaCommandHandlerAsync<TSagaCommand>(SagaCommand command)
        where TSagaCommand : class, ISagaCommand
    {
        if (command is TSagaCommand sagaCommand &&
            SagaCommandHandlers.OfType<ISagaCommandHandler<TSagaCommand>>().FirstOrDefault() is { } handler)
        {
            await handler.HandleCommandAsync(sagaCommand);
            return true;
        }

        return false;
    }
    
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