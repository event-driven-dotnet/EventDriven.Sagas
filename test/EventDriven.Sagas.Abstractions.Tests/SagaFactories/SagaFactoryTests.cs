using System;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Abstractions.Tests.Helpers;
using EventDriven.Sagas.Abstractions.Tests.SagaFactories.Fakes;
using EventDriven.Sagas.Configuration.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactories;

public class SagaFactoryTests
{
    [Theory]
    [InlineData(SagaType.Basic)]
    [InlineData(SagaType.Configurable)]
    [InlineData(SagaType.Persistable)]
    public async Task SagaPool_ShouldCreateSaga(SagaType sagaType)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeSagaFactory(sagaType);
        var provider = services.BuildServiceProvider();
        var sagaPool = GetSagaPool(provider, sagaType);
        var saga = await sagaPool.CreateSagaAsync();
        
        // Act
        await saga.StartSagaAsync();

        // Assert
        var expectedState = sagaType == SagaType.Basic ? SagaState.Executing : SagaState.Executed;
        var expectedInfo = sagaType == SagaType.Basic ? null : FakeSaga.SuccessState;
        Assert.Equal(expectedState, saga.State);
        Assert.Equal(expectedInfo, saga.StateInfo);
    }

    private ISagaPool GetSagaPool(IServiceProvider provider, SagaType sagaType)
    {
        var factory = provider.GetRequiredService<ISagaFactory<Abstractions.Saga>>();
        var resultDispatchers = provider.GetServices<ISagaCommandResultDispatcher>();
        switch (sagaType)
        {
            case SagaType.Configurable:
                var sagaPool1 = new InMemorySagaPool<FakeConfigurableSaga>((ConfigurableSagaFactory<FakeConfigurableSaga>)factory,
                    resultDispatchers,true, true);
                return sagaPool1;
            case SagaType.Persistable:
                var sagaPool2 = new InMemorySagaPool<FakePersistableSaga>((PersistableSagaFactory<FakePersistableSaga>)factory,
                    resultDispatchers,true, true);
                return sagaPool2;
            default:
                var sagaPool3 = new InMemorySagaPool<FakeSaga>((SagaFactory<FakeSaga>)factory,
                    resultDispatchers,true, true);
                return sagaPool3;
        }
    }
}