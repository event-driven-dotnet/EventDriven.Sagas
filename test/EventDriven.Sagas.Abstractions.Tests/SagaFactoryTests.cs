using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Tests.SagaFactoryFakes;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventDriven.Sagas.Abstractions.Tests;

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
        services.AddSingleton<ISagaCommandResultEvaluator<string, string>, FakeCommandResultEvaluator>();
        services.AddSingleton<ISagaCommandHandler<FakeEntity, FakeSagaCommand>, FakeSagaCommandHandler>();
        services.AddSingleton(sp =>
        {
            var sagaConfigOptions = new SagaConfigurationOptions();
            var dispatcher = sp.GetRequiredService<FakeSagaCommandDispatcher>();
            var evaluator = sp.GetRequiredService<ISagaCommandResultEvaluator<string, string>>();
            ISagaFactory<Saga> factory;
            switch (sagaType)
            {
                case SagaType.Configurable:
                    factory = new ConfigurableSagaFactory
                        <FakeConfigurableSaga, FakeSagaCommand, FakeEntity>(
                            dispatcher, evaluator, sagaConfigOptions, null!);
                    break;
                case SagaType.Persistable:
                    factory = new PersistableSagaFactory
                        <FakePersistableSaga, FakeSagaCommand, FakeEntity>(
                            dispatcher, evaluator, sagaConfigOptions, null!, null!);
                    break;
                default:
                    factory = new SagaFactory
                        <FakeSaga, FakeSagaCommand, FakeEntity>(dispatcher, evaluator);
                    break;
            }
            return factory;
        });
        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<ISagaFactory<Saga>>();
            var saga = factory.CreateSaga();
            return saga;
        });

        // Act
        var provider = services.BuildServiceProvider();
        var saga = provider.GetRequiredService<Saga>();
        await saga.StartSagaAsync();

        // Assert
        var expectedState = sagaType == SagaType.Basic ? SagaState.Executing : SagaState.Executed;
        var expectedInfo = sagaType == SagaType.Basic ? null : FakeSaga.SuccessState;
        Assert.Equal(expectedState, saga.State);
        Assert.Equal(expectedInfo, saga.StateInfo);
    }
}