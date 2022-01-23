using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Sagas.Commands;

public record SetOrderStateInitial(Guid? EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);
