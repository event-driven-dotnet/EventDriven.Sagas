using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.Dispatchers;

public class OrderCommandDispatcher : ISagaCommandDispatcher
{
    private readonly ICommandHandler<Order, SetOrderStatePending> _commandHandler;

    public OrderCommandDispatcher(ICommandHandler<Order, SetOrderStatePending> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task DispatchAsync(ISagaCommand command, bool compensating)
    {
        // Based on command name, dispatch command to handler
        switch (command.Name)
        {
            case "SetStatePending":
                await _commandHandler.Handle(new SetOrderStatePending
                {
                    // TODO: Pass order or order id?
                    Name = command.Name,
                    Result = OrderState.Pending
                });
                break;
        }
    }
}