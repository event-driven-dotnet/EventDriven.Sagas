using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Configuration.Abstractions;

namespace SagaFactoryTest;

public class FakeSaga : ConfigurableSaga<FakeEntity>
{
    public FakeSaga(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        ICommandResultEvaluator commandResultEvaluator) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
    }

    protected override async Task ExecuteCurrentActionAsync() =>
        await SagaCommandDispatcher.DispatchAsync(new FakeSagaCommand
        {
            Name = "Fake Saga Command"
        }, false);

    protected override Task ExecuteCurrentCompensatingActionAsync() => throw new NotImplementedException();
    public override Task ProcessCommandResultAsync(FakeEntity commandResult, bool compensating)
    {
        Console.WriteLine($"Processing command result.");
        return Task.CompletedTask;
    }
}