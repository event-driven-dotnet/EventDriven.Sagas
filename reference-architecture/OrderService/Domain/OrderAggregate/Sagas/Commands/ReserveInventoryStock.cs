using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Commands;

public record ReserveInventoryStock(Guid? EntityId = default) : SagaCommand(EntityId);