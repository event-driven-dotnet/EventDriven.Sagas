using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;

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
        if (SagaCommandHandlers.OfType<ISagaCommandHandler<FakeSagaCommand>>().FirstOrDefault() is { } handler)
        {
            await handler.HandleCommandAsync(new FakeSagaCommand
            {
                Name = command.Name
            });
        }
    }
}