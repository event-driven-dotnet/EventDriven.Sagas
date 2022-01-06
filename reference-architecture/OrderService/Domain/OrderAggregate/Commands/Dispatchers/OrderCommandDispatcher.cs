using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;

namespace OrderService.Domain.OrderAggregate.Commands.Dispatchers;

public class OrderCommandDispatcher : ISagaCommandDispatcher
{
    private readonly ICommandHandler<Order, SetOrderStatePending> _commandHandler;

    public OrderCommandDispatcher(ICommandHandler<Order, SetOrderStatePending> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task DispatchAsync(SagaCommand command, bool compensating)
    {
        // Based on command name, dispatch command to handler
        switch (command.Name)
        {
            case "SetStatePending":
                await _commandHandler.Handle(new SetOrderStatePending(command.EntityId)
                {
                    Name = command.Name,
                    Result = OrderState.Pending
                });
                break;
        }
    }
}