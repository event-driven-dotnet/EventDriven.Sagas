namespace InventoryService.Domain.InventoryAggregate.Events;

public record InventoryReserveSucceeded(Guid EntityId, int AmountRequested) : InventoryReserved(EntityId, AmountRequested);