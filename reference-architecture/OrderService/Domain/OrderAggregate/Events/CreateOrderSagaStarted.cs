using EventDriven.DDD.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events;

public record CreateOrderSagaStarted(Guid EntityId) : DomainEvent(EntityId);