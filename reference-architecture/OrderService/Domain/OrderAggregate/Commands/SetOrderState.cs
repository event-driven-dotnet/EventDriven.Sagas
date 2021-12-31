using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions;

namespace OrderService.Domain.OrderAggregate.Commands;

public record SetOrderState :
    Command,
    ISagaCommand<OrderState, OrderState>
{
    public string? Name { get; set; } = "SetStatePending";
    public OrderState Payload { get; set; }
    public OrderState ExpectedResult { get; set; }
}