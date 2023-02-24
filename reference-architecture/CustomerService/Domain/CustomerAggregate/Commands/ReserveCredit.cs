using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record ReserveCredit(Guid EntityId, decimal AmountRequested, Guid CorrelationId) :
    Command<Customer>(null, EntityId);