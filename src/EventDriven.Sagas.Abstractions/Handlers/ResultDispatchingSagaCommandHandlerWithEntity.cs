using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Pools;

namespace EventDriven.Sagas.Abstractions.Handlers;

/// <summary>
/// Command handler that can dispatch command results.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Command type.</typeparam>
public abstract class ResultDispatchingSagaCommandHandlerWithEntity<TEntity, TSagaCommand> :
    ISagaCommandHandler<TEntity, TSagaCommand>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public abstract Task<TEntity> HandleCommandAsync(TSagaCommand command);
}

/// <summary>
/// Command handler that can dispatch command results.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Command type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingSagaCommandHandler<TSaga, TEntity, TSagaCommand, TResult> :
    ResultDispatchingSagaCommandHandlerWithEntity<TEntity, TSagaCommand>,
    ISagaCommandResultDispatcher<TResult>
    where TSaga : Saga
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public Type? SagaType { get; set; }

    /// <summary>
    /// Saga pool.
    /// </summary>
    public ISagaPool SagaPool { get; set; } = null!;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected ResultDispatchingSagaCommandHandler()
    {
        SagaType = typeof(TSaga);
    }

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating, Guid sagaId)
    {
        // Use Saga Pool to get saga
        var sagaPool = (SagaPool<TSaga>)SagaPool;
        var saga = sagaPool[sagaId];
        if (saga is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}