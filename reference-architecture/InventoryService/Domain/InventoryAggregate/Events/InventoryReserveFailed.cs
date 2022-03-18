namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryReserveFailed(Guid EntityId, int AmountRequested) : InventoryReserved(EntityId, AmountRequested);