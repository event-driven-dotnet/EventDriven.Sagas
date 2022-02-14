using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Sagas.CreateOrder.Commands;

public record CreateOrder : SagaCommand<OrderState, OrderState>;
