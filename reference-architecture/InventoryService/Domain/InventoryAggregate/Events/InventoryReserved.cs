using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryReserved(Guid EntityId, int AmountRequested) : DomainEvent(EntityId);