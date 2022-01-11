using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Factories;

namespace EventDriven.Sagas.Configuration.Abstractions.Factories;

/// <summary>
/// Factory for creating configurable saga instances.
/// </summary>
public class ConfigurableSagaFactory<TSaga, TEntity, TSagaCommand>
    : SagaFactory<TSaga, TEntity, TSagaCommand>
    where TSaga : ConfigurableSaga<TEntity>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    public ConfigurableSagaFactory(
        ISagaCommandDispatcher<TEntity, TSagaCommand> sagaCommandDispatcher, 
        ICommandResultEvaluator commandResultEvaluator, 
        ISagaCommandHandler<TEntity, TSagaCommand> sagaCommandHandler) : 
        base(sagaCommandDispatcher, commandResultEvaluator, sagaCommandHandler)
    {
    }

    /// <inheritdoc />
    public override Saga? CreateSaga()
    {
        var saga = base.CreateSaga();
        if (saga is TSaga processor)
            SagaCommandHandler.CommandResultProcessor = processor;
        return saga;
    }
}