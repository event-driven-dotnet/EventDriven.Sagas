using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Commands;

public record GetOrderState(Guid? EntityId = default) : SagaCommand(EntityId);
