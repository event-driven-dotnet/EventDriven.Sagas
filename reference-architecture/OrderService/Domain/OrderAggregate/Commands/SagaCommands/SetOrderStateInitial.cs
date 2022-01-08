using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStateInitial(Guid EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);
