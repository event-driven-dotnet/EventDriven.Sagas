namespace Common.Integration.Models;

public record ProductInventoryReserveResponse(Guid InventoryId, int AmountRequested, int AmountAvailable, bool Success);