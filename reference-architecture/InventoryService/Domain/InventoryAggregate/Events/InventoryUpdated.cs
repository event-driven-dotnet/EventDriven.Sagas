using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryUpdated(Inventory Inventory) : DomainEvent(Inventory.Id);
