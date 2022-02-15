using EventDriven.DDD.Abstractions.Events;

namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryCreated(Inventory Inventory) : DomainEvent(Inventory.Id);
