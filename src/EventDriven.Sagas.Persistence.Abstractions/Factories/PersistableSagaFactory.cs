using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions.Factories;

/// <inheritdoc />
public class PersistableSagaFactory<TSaga, TSagaCommand, TEntity>
    : SagaFactory<TSaga, TSagaCommand, TEntity>
    where TSaga : PersistableSaga<TEntity>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    private readonly SagaConfigurationOptions _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;
    private readonly ISagaSnapshotRepository _sagaSnapshotRepository;

    /// <inheritdoc />
    public PersistableSagaFactory(
        ISagaCommandDispatcher<TEntity, TSagaCommand> sagaCommandDispatcher, 
        ISagaCommandResultEvaluator commandResultEvaluator,
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository,
        ISagaSnapshotRepository sagaSnapshotRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
        _sagaSnapshotRepository = sagaSnapshotRepository;
    }

    /// <inheritdoc />
    public override TSaga CreateSaga()
    {
        var saga = base.CreateSaga();
        saga.SagaConfigOptions = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        saga.SagaSnapshotRepository = _sagaSnapshotRepository;
        var dispatcher = (ISagaCommandDispatcher<TEntity, TSagaCommand>)SagaCommandDispatcher;
        dispatcher.SagaCommandHandler.CommandResultProcessor = saga;
        return saga;
    }
}