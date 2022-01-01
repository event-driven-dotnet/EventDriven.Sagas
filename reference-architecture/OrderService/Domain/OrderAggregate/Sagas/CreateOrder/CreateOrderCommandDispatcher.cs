using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.CreateOrder;

public class CreateOrderCommandDispatcher : ISagaCommandDispatcher
{
    private readonly OrderCommandHandler _orderCommandHandler;

    public CreateOrderCommandDispatcher(OrderCommandHandler orderCommandHandler)
    {
        _orderCommandHandler = orderCommandHandler;
    }

    public async Task DispatchAsync(ISagaCommand command, bool compensating)
    {
        // Based on command name, dispatch command to handler
        switch (command.Name)
        {
            case "SetStatePending":
                await _orderCommandHandler.Handle(new SetOrderStatePending
                {
                    //Order = command.
                    Name = command.Name,
                    Payload = OrderState.Pending
                });
                break;
        }
    }
}