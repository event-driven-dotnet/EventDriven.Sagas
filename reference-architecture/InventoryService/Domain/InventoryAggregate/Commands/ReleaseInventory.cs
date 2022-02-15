using EventDriven.DDD.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record ReleaseInventory(Guid EntityId, int AmountReleased) : ICommand;