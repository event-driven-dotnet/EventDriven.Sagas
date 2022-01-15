using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
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
        var handler = GetSagaCommandHandlerByCommandType<CreateOrder>();
        if (handler != null && command is CreateOrder createOrder)
            await handler.HandleCommandAsync(createOrder);
    }
}