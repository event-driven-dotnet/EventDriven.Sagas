using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class OrderCommandHandler : ISagaCommandHandler
{
    private readonly Order _order;
    private readonly ISagaCommandResultHandler<Order> _resultProcessor;

    public OrderCommandHandler(Order order, ISagaCommandResultHandler<Order> resultProcessor)
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
            await _resultProcessor.HandleCommandResultAsync(_order, compensating);
        }
    }
}