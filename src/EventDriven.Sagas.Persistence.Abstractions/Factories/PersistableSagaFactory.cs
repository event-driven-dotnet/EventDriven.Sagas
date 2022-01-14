using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Factories;
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
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository,
        ISagaSnapshotRepository sagaSnapshotRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluators, commandResultDispatchers)
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
        return saga;
    }
}