using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSagaCommandDispatcher : SagaCommandDispatcher<FakeEntity, FakeSagaCommand>
{

    public FakeSagaCommandDispatcher(
        ISagaCommandHandler<FakeEntity, FakeSagaCommand> sagaCommandHandler)
    {
        SagaCommandHandler = sagaCommandHandler;
    }

    public override async Task DispatchAsync(SagaCommand command, bool compensating)
    {
        await SagaCommandHandler.Handle(new FakeSagaCommand
        {
            Name = command.Name
        });
    }
}