using EventDriven.Sagas.Abstractions.Commands;
using Integration.Models;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReleaseProductInventory(Guid InventoryId, int AmountRequested) :
    SagaCommand<ProductInventoryReleaseResponse, ProductInventoryReleaseResponse>
{
    public Guid InventoryId { get; set; } = InventoryId;
    public int AmountRequested { get; set; } = AmountRequested;
}