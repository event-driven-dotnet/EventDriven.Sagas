using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions.Factories;

/// <inheritdoc />
public class PersistableSagaFactory<TSaga>
    : SagaFactory<TSaga>
    where TSaga : PersistableSaga, ISagaCommandResultHandler
{
    private readonly SagaConfigurationOptions _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;
    private readonly ISagaSnapshotRepository _sagaSnapshotRepository;

    /// <inheritdoc />
    public PersistableSagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers,
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository,
        ISagaSnapshotRepository sagaSnapshotRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluators,
            commandResultDispatchers, checkLockCommandHandlers)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
        _sagaSnapshotRepository = sagaSnapshotRepository;
    }

    /// <param name="overrideLock"></param>
    /// <inheritdoc />
    public override TSaga CreateSaga(bool overrideLock)
    {
        var saga = base.CreateSaga(overrideLock);
        saga.SagaConfigOptions = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        saga.SagaSnapshotRepository = _sagaSnapshotRepository;
        return saga;
    }
}