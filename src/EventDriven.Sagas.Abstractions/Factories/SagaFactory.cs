using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Factories;

/// <summary>
/// Factory for creating saga instances.
/// </summary>
public abstract class SagaFactory
{
    /// <summary>
    /// Create a saga.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    public abstract Saga? CreateSaga();
}

/// <inheritdoc />
public abstract class SagaFactory<TSaga, TEntity, TSagaCommand> : SagaFactory
    where TSaga : Saga
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    protected readonly ISagaCommandDispatcher<TEntity, TSagaCommand> SagaCommandDispatcher;
    protected readonly ICommandResultEvaluator CommandResultEvaluator;
    protected readonly ISagaCommandHandler<TEntity, TSagaCommand> SagaCommandHandler;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandDispatcher">Saga command dispatcher.</param>
    /// <param name="commandResultEvaluator">Command result evaluator.</param>
    /// <param name="sagaCommandHandler">Saga command handler.</param>
    protected SagaFactory(
        ISagaCommandDispatcher<TEntity, TSagaCommand> sagaCommandDispatcher,
        ICommandResultEvaluator commandResultEvaluator,
        ISagaCommandHandler<TEntity, TSagaCommand> sagaCommandHandler)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        CommandResultEvaluator = commandResultEvaluator;
        SagaCommandHandler = sagaCommandHandler;
    }
    /// <inheritdoc />
    public override Saga? CreateSaga()
    {
        SagaCommandDispatcher.SagaCommandHandler = SagaCommandHandler;
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, CommandResultEvaluator);
        return saga;
    }
}