using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record SetOrderStatePending : Command,
    ISagaCommand<OrderState, OrderState>
{
    public Order Order { get; set; } = null!;
    public string? Name { get; set; } = "SetStatePending";
    public OrderState Result { get; set; }
    public OrderState ExpectedResult { get; set; }
}