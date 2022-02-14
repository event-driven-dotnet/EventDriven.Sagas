using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Sagas.CreateOrder.Commands;

public record SetOrderStateInitial(Guid? EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);
