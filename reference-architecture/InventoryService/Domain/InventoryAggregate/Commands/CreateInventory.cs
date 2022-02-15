using EventDriven.DDD.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record CreateInventory(Inventory Entity) : Command<Inventory>(Entity);