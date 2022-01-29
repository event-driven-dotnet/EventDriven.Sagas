using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReserveSucceeded(Guid EntityId, decimal AmountRequested) : DomainEvent(EntityId);