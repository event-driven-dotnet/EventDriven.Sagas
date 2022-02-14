using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReserveInventoryStock(Guid? EntityId = default) : SagaCommand(EntityId);