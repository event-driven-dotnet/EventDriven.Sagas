// See https://aka.ms/new-console-template for more information

using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Configuration.Abstractions.Factories;
using Microsoft.Extensions.DependencyInjection;
using SagaFactoryTest;

Console.WriteLine("Saga factory test ...");

var services = new ServiceCollection();
services.AddSingleton<FakeSagaCommandDispatcher>();
services.AddSingleton<FakeCommandResultEvaluator>();
services.AddSingleton<ISagaCommandHandler<FakeEntity, FakeSagaCommand>, FakeSagaCommandHandler>();
services.AddSingleton<SagaFactory>(sp =>
{
    var dispatcher = sp.GetRequiredService<FakeSagaCommandDispatcher>();
    var evaluator = sp.GetRequiredService<FakeCommandResultEvaluator>();
    var handler = sp.GetRequiredService<ISagaCommandHandler<FakeEntity, FakeSagaCommand>>();
    var factory = new ConfigurableSagaFactory
        <FakeSaga, FakeEntity, FakeSagaCommand>(
            dispatcher, evaluator, handler);
    return factory;
});

services.AddSingleton<Saga>(sp =>
{
    var factory = sp.GetRequiredService<SagaFactory>();
    var saga = factory.CreateSaga();
    return saga;
});

var provider = services.BuildServiceProvider();
var saga = provider.GetRequiredService<Saga>();
Console.WriteLine($"Starting saga: {saga.Id}");
await saga.StartSagaAsync();
Console.WriteLine($"Saga completed.");
