using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;

namespace OrderService.Domain.OrderAggregate.Commands.Dispatchers;

public class OrderCommandDispatcher : SagaCommandDispatcher<Order, SetOrderStatePending>
{
    public OrderCommandDispatcher(ISagaCommandHandler<Order, SetOrderStatePending> sagaCommandHandler)
    {
        SagaCommandHandler = sagaCommandHandler;
    }

    public override async Task DispatchAsync(SagaCommand command, bool compensating)
    {
        // Based on command name, dispatch command to handler
        if (string.Equals(command.Name, typeof(SetOrderStatePending).FullName, StringComparison.OrdinalIgnoreCase))
        {
            await SagaCommandHandler.Handle(new SetOrderStatePending(command.EntityId)
            {
                Name = command.Name,
                Result = OrderState.Pending
            });
        }
    }
}