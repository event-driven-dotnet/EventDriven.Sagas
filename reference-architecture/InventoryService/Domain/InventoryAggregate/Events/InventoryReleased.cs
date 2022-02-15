using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryReleased(Guid EntityId, int AmountReleased) : DomainEvent(EntityId);