using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class OrderCommandHandler : ISagaCommandHandler
{
    private readonly Order _order;
    private readonly ICommandResultProcessor<Order> _resultProcessor;

    public OrderCommandHandler(Order order, ICommandResultProcessor<Order> resultProcessor)
    {
        _order = order;
        _resultProcessor = resultProcessor;
    }

    public async Task HandleAsync(ISagaCommand command, bool compensating)
    {
        if (command.Name!.StartsWith("SetState"))
        {
            var payload = ((FakeCommand)command).Result;
            _order.State = payload;
            await _resultProcessor.ProcessCommandResultAsync(_order, compensating);
        }
    }
}