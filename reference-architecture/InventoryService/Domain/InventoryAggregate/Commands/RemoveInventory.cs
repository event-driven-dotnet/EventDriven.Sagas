using EventDriven.CQRS.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record RemoveInventory(Guid EntityId) : Command(EntityId);