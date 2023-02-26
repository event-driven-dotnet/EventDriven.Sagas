using EventDriven.CQRS.Abstractions.Commands;

namespace InventoryService.Domain.InventoryAggregate.Commands;

public record ReserveInventory(Guid EntityId, int AmountRequested, Guid CorrelationId) :
    Command<Inventory>(null, EntityId);