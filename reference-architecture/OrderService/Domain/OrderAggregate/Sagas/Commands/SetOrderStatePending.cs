using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Commands;

public record SetOrderStatePending(Guid EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);
