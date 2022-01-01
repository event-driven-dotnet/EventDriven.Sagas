using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record GetOrderState(Guid EntityId) : Command(EntityId);