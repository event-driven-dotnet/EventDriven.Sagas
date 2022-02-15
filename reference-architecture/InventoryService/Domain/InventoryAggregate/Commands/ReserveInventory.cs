using EventDriven.DDD.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record ReserveInventory(Guid EntityId, int AmountRequested) : ICommand;