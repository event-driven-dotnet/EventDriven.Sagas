using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Dispatchers;

public class OrderCommandDispatcher : SagaCommandDispatcher
{
    public OrderCommandDispatcher(IEnumerable<ISagaCommandHandler> sagaCommandHandlers) :
        base(sagaCommandHandlers)
    {
    }

    public override async Task DispatchCommandAsync(SagaCommand command, bool compensating)
    {
        var handler = GetSagaCommandHandlerByCommandType<SetOrderStatePending>();
        if (handler != null)
            await handler.HandleCommandAsync(new SetOrderStatePending(command.EntityId)
            {
                Name = command.Name,
                Result = OrderState.Pending
            });
    }
}