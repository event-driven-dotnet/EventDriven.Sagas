using EventDriven.Sagas.Abstractions.Commands;
using Integration.Models;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReserveProductInventory(Guid InventoryId, int AmountRequested) :
    SagaCommand<ProductInventoryReserveResponse, ProductInventoryReserveResponse>
{
    public Guid InventoryId { get; set; } = InventoryId;
    public int AmountRequested { get; set; } = AmountRequested;
}