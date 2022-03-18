namespace Common.Integration.Models;

public record ProductInventoryReleaseResponse(Guid InventoryId, decimal AmountRequested, decimal AmountRemaining, bool Success);