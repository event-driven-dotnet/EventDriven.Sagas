using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.Configuration.Abstractions.Factories;

/// <inheritdoc />
public class ConfigurableSagaFactory<TSaga>
    : SagaFactory<TSaga>
    where TSaga : ConfigurableSaga, ISagaCommandResultHandler
{
    private readonly SagaConfigurationOptions _sagaConfigOptions;
    private readonly ISagaConfigRepository _sagaConfigRepository;

    /// <inheritdoc />
    public ConfigurableSagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers,
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluators,
            commandResultDispatchers, checkLockCommandHandlers)
    {
        _sagaConfigOptions = sagaConfigOptions;
        _sagaConfigRepository = sagaConfigRepository;
    }

    /// <param name="overrideLock"></param>
    /// <inheritdoc />
    public override TSaga CreateSaga(bool overrideLock)
    {
        var saga = base.CreateSaga(overrideLock);
        saga.SagaConfigOptions = _sagaConfigOptions;
        saga.SagaConfigRepository = _sagaConfigRepository;
        return saga;
    }
}