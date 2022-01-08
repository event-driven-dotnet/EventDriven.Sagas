using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveCustomerCredit : SagaCommand
{
    public ReserveCustomerCredit(Guid orderId) : base(orderId)
    {
    }
}