using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReserveFailed(Guid EntityId, decimal AmountRequested) : DomainEvent(EntityId);