using EventDriven.CQRS.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record ReleaseInventory(Guid EntityId, int AmountReleased) : Command<Inventory>(null, EntityId);