using System.Linq;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;
using EventDriven.Sagas.Configuration.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory;

public class SagaFactoryTests
{
    [Theory]
    [InlineData(SagaType.Basic)]
    [InlineData(SagaType.Configurable)]
    [InlineData(SagaType.Persistable)]
    public async Task SagaFactoryShouldCreateConfigurableSaga(SagaType sagaType)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<FakeSagaCommandDispatcher>();
        services.AddSingleton<FakeSagaCommandHandler<FakeSaga>>();
        services.AddSingleton<FakeSagaCommandHandler<FakeConfigurableSaga>>();
        services.AddSingleton<FakeSagaCommandHandler<FakePersistableSaga>>();
        services.AddSingleton<FakeSagaCommandHandler<FakeSaga>>();
        services.AddSingleton<ISagaCommandResultEvaluator, FakeCommandResultEvaluator>();
        switch (sagaType)
        {
            case SagaType.Basic:
                services.AddSingleton<ISagaCommandResultDispatcher, FakeSagaCommandHandler<FakeSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakeSaga>>());
                services.AddSingleton<ISagaCommandHandler, FakeSagaCommandHandler<FakeSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakeSaga>>());
                break;
            case SagaType.Configurable:
                services.AddSingleton<ISagaCommandResultDispatcher, FakeSagaCommandHandler<FakeConfigurableSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakeConfigurableSaga>>());
                services.AddSingleton<ISagaCommandHandler, FakeSagaCommandHandler<FakeConfigurableSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakeConfigurableSaga>>());
                break;
            case SagaType.Persistable:
                services.AddSingleton<ISagaCommandResultDispatcher, FakeSagaCommandHandler<FakePersistableSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakePersistableSaga>>());
                services.AddSingleton<ISagaCommandHandler, FakeSagaCommandHandler<FakePersistableSaga>>(sp =>
                    sp.GetRequiredService<FakeSagaCommandHandler<FakePersistableSaga>>());
                break;
        }
        services.AddSingleton(sp =>
        {
            var sagaConfigOptions = new FakeSagaConfigSettings();
            var dispatcher = sp.GetRequiredService<FakeSagaCommandDispatcher>();
            var evaluators = sp.GetServices<ISagaCommandResultEvaluator>();
            ISagaFactory<Abstractions.Saga> factory;
            switch (sagaType)
            {
                case SagaType.Configurable:
                    factory = new ConfigurableSagaFactory<FakeConfigurableSaga>(
                        dispatcher, evaluators, Enumerable.Empty<ICheckSagaLockCommandHandler>(), 
                        sagaConfigOptions, null!);
                    break;
                case SagaType.Persistable:
                    factory = new PersistableSagaFactory<FakePersistableSaga>(
                        dispatcher, evaluators, Enumerable.Empty<ICheckSagaLockCommandHandler>(), 
                        sagaConfigOptions, null!, null!);
                    break;
                default:
                    factory = new SagaFactory<FakeSaga>(
                        dispatcher, evaluators, Enumerable.Empty<ICheckSagaLockCommandHandler>());
                    break;
            }
            return factory;
        });

        Abstractions.Saga saga;
        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ISagaFactory<Abstractions.Saga>>();
        var resultDispatchers = provider.GetServices<ISagaCommandResultDispatcher>();
        switch (sagaType)
        {
            case SagaType.Configurable:
                var sagaPool1 = new SagaPool<FakeConfigurableSaga>((ConfigurableSagaFactory<FakeConfigurableSaga>)factory,
                    resultDispatchers,true);
                saga = sagaPool1.CreateSaga();
                break;
            case SagaType.Persistable:
                var sagaPool2 = new SagaPool<FakePersistableSaga>((PersistableSagaFactory<FakePersistableSaga>)factory,
                    resultDispatchers,true);
                saga = sagaPool2.CreateSaga();
                break;
            default:
                var sagaPool3 = new SagaPool<FakeSaga>((SagaFactory<FakeSaga>)factory,
                    resultDispatchers,true);
                saga = sagaPool3.CreateSaga();
                break;
        }

        // Act
        await saga.StartSagaAsync();

        // Assert
        var expectedState = sagaType == SagaType.Basic ? SagaState.Executing : SagaState.Executed;
        var expectedInfo = sagaType == SagaType.Basic ? null : FakeSaga.SuccessState;
        Assert.Equal(expectedState, saga.State);
        Assert.Equal(expectedInfo, saga.StateInfo);
    }
}