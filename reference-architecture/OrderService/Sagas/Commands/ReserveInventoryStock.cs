using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.Commands;

public record ReserveInventoryStock(Guid? EntityId = default) : SagaCommand(EntityId);