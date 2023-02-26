using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record ReleaseCredit(Guid EntityId, decimal AmountReleased, Guid CorrelationId) :
    Command<Customer>(null, EntityId);