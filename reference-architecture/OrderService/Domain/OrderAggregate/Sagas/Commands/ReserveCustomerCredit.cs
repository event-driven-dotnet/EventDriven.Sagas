using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Commands;

public record ReserveCustomerCredit(Guid EntityId = default) : SagaCommand(EntityId);