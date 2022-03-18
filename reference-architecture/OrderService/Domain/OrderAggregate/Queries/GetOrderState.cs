using EventDriven.CQRS.Abstractions.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrderState(Guid Id) : Query<OrderState?>;