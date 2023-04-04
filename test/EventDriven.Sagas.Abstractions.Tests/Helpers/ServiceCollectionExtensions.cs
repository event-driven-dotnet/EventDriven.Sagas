using System.Linq;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Tests.SagaFactories.Fakes;
using EventDriven.Sagas.Configuration.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions.Tests.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFakeSagaFactory(this IServiceCollection services, SagaType sagaType)
    {
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
        return services;
    }
}