using EventDriven.Sagas.Abstractions.Commands;

namespace SagaFactoryTest;

public class FakeSagaCommandDispatcher : ISagaCommandDispatcher<FakeEntity, FakeSagaCommand>
{

    public FakeSagaCommandDispatcher(
        ISagaCommandHandler<FakeEntity, FakeSagaCommand> sagaCommandHandler)
    {
        SagaCommandHandler = sagaCommandHandler;
    }

    public async Task DispatchAsync(SagaCommand command, bool compensating)
    {
        Console.WriteLine("Dispatching saga command.");
        await SagaCommandHandler.Handle(new FakeSagaCommand
        {
            Name = command.Name
        });
    }

    public ISagaCommandHandler<FakeEntity, FakeSagaCommand> SagaCommandHandler { get; set; }
}