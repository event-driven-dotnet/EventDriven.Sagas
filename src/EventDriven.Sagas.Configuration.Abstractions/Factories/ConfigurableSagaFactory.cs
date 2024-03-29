using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.Configuration.Abstractions.Factories;

/// <inheritdoc />
public class ConfigurableSagaFactory<TSaga>
    : SagaFactory<TSaga>
    where TSaga : ConfigurableSaga, ISagaCommandResultHandler
{
    private readonly SagaConfigSettings _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;

    /// <inheritdoc />
    public ConfigurableSagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers,
        SagaConfigSettings sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluators, checkLockCommandHandlers)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
    }

    /// <inheritdoc />
    public override TSaga CreateSaga(ISagaPool sagaPool, bool overrideLock, bool enableSagaSnapshots)
    {
        var saga = base.CreateSaga(sagaPool, overrideLock, enableSagaSnapshots);
        saga.SagaConfigSettings = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        return saga;
    }
}