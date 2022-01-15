using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Commands;

public record CreateOrder : SagaCommand<OrderState, OrderState>;
