using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.Configuration.Abstractions.Factories;

/// <inheritdoc />
public class ConfigurableSagaFactory<TSaga, TSagaCommand, TEntity>
    : SagaFactory<TSaga, TSagaCommand, TEntity>
    where TSaga : ConfigurableSaga<TEntity>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    private readonly SagaConfigurationOptions _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;

    /// <inheritdoc />
    public ConfigurableSagaFactory(
        ISagaCommandDispatcher<TEntity, TSagaCommand> sagaCommandDispatcher, 
        ISagaCommandResultEvaluator commandResultEvaluator,
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
    }

    /// <inheritdoc />
    public override TSaga CreateSaga()
    {
        var saga = base.CreateSaga();
        saga.SagaConfigOptions = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        var dispatcher = (ISagaCommandDispatcher<TEntity, TSagaCommand>)SagaCommandDispatcher;
        dispatcher.SagaCommandHandler.CommandResultProcessor = saga;
        return saga;
    }
}