using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.CreateOrder.Commands;

public record GetOrderState(Guid? EntityId = default) : SagaCommand(EntityId);
