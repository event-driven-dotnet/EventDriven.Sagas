using System.Collections.Generic;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSagaCommandDispatcher : SagaCommandDispatcher
{
    public FakeSagaCommandDispatcher(
        IEnumerable<ISagaCommandHandler> sagaCommandHandlers) :
        base(sagaCommandHandlers)
    {
    }

    public override async Task DispatchCommandAsync(SagaCommand command, bool compensating)
    {
        var handler = GetSagaCommandHandlerByCommandType<FakeSagaCommand>(command);
        if (handler != null)
            await handler.HandleCommandAsync(new FakeSagaCommand
            {
                Name = command.Name
            });
    }
}