using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStatePending : SagaCommand<OrderState, OrderState>;
