using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveCustomerCredit(Guid EntityId = default) : SagaCommand(EntityId);