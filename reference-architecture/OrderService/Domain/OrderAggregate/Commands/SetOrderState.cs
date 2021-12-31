using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record SetOrderState :
    Command,
    ISagaCommand<OrderState, OrderState>
{
    public string? Name { get; set; } = "SetStatePending";
    public OrderState Payload { get; set; }
    public OrderState ExpectedResult { get; set; }
}