using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryReserveFailed(Guid EntityId, int AmountRequested) : DomainEvent(EntityId);