namespace Integration.Models;

public record ProductInventoryReserveRequest(Guid InventoryId, int AmountReserved);