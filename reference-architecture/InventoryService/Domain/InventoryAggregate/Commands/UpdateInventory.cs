using EventDriven.DDD.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record UpdateInventory(Inventory Entity) : Command<Inventory>(Entity);