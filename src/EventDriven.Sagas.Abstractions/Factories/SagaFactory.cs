using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Factories;

/// <summary>
/// Factory for creating saga instances.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
public class SagaFactory<TSaga> : ISagaFactory<TSaga>
    where TSaga : Saga
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandDispatcher">Saga command dispatcher.</param>
    /// <param name="sagaCommandResultEvaluator">Saga command result evaluator.</param>
    public SagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher,
        ISagaCommandResultEvaluator sagaCommandResultEvaluator)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        SagaCommandResultEvaluator = sagaCommandResultEvaluator;
    }

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public virtual ISagaCommandDispatcher SagaCommandDispatcher { get; }

    /// <summary>
    /// Command result evaluator.
    /// </summary>
    public virtual ISagaCommandResultEvaluator SagaCommandResultEvaluator { get; }

    /// <inheritdoc />
    public virtual TSaga CreateSaga()
    {
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, SagaCommandResultEvaluator);
        if (saga == null)
            throw new Exception($"Unable to create instance of {typeof(TSaga).Name}");
        return saga;
    }
}

/// <summary>
/// Factory for creating saga instances.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
/// <typeparam name="TEntity">Entity type.</typeparam>
public class SagaFactory<TSaga, TSagaCommand, TEntity> : SagaFactory<TSaga>
    where TSaga : Saga
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public SagaFactory(
        ISagaCommandDispatcher<TEntity, TSagaCommand> sagaCommandDispatcher,
        ISagaCommandResultEvaluator commandResultEvaluator) :
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
    }

    // /// <inheritdoc />
    // public override TSaga CreateSaga()
    // {
    //     var handler = (ISagaCommandHandler<TEntity, TSagaCommand>)SagaCommandHandler;
    //     var dispatcher = (ISagaCommandDispatcher<TEntity, TSagaCommand>)SagaCommandDispatcher;
    //     dispatcher.SagaCommandHandler = handler;
    //     return base.CreateSaga();
    // }
}