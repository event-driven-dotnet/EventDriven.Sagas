using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStateInitial : SagaCommand<OrderState, OrderState>
{
    public SetOrderStateInitial(Guid orderId) : base(orderId)
    {
    }
}
