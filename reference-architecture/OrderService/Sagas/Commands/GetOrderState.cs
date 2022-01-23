using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.Commands;

public record GetOrderState(Guid? EntityId = default) : SagaCommand(EntityId);
