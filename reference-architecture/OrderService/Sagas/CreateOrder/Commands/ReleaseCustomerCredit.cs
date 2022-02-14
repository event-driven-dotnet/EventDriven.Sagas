using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReleaseCustomerCredit(Guid CustomerId, decimal CreditReleased) : SagaCommand;