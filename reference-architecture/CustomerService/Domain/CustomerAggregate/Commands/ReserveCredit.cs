using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record ReserveCredit(Guid EntityId, decimal AmountRequested) : Command<Customer>(null, EntityId);