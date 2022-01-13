using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Factories;
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
        ISagaCommandResultEvaluator commandResultEvaluator,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        SagaConfigurationOptions sagaConfigOptions,
        ISagaConfigRepository sagaConfigRepository) : 
        base(sagaCommandDispatcher, commandResultEvaluator, commandResultDispatchers)
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
        return saga;
    }
}