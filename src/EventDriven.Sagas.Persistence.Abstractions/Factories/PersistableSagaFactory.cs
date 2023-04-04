using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions.Factories;

/// <inheritdoc />
public class PersistableSagaFactory<TSaga>
    : SagaFactory<TSaga>
    where TSaga : PersistableSaga, ISagaCommandResultHandler
{
    private readonly ISagaConfigSettings _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;
    private readonly ISagaSnapshotRepository _sagaSnapshotRepository;

    /// <inheritdoc />
    public PersistableSagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers,
        ISagaConfigSettings sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository,
        ISagaSnapshotRepository sagaSnapshotRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluators, checkLockCommandHandlers)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
        _sagaSnapshotRepository = sagaSnapshotRepository;
    }

    /// <inheritdoc />
    public override TSaga CreateSaga(ISagaPool sagaPool, bool overrideLock)
    {
        var saga = base.CreateSaga(sagaPool, overrideLock);
        saga.SagaConfigSettings = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        saga.SagaSnapshotRepository = _sagaSnapshotRepository;
        return saga;
    }
}