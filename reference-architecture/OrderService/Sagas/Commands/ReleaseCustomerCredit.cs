using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.Commands;

public record ReleaseCustomerCredit(Guid CustomerId, decimal CreditReleased) : SagaCommand;