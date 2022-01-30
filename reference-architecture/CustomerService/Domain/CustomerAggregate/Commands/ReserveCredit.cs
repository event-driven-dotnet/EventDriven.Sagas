using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record ReserveCredit(Guid EntityId, decimal CreditRequested) : ICommand;