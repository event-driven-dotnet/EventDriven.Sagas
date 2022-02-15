using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryRemoved(Guid EntityId) : DomainEvent(EntityId);
