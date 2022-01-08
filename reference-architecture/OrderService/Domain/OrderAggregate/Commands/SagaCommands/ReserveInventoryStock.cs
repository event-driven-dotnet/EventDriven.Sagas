using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveInventoryStock : SagaCommand
{
    public ReserveInventoryStock(Guid orderId) : base(orderId)
    {
    }
}