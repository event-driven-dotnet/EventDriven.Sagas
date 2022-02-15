namespace Integration.Models;

public record ProductInventoryReleaseRequest(Guid InventoryId, int AmountReleased);