using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReserved(Guid EntityId, decimal AmountReserved) : DomainEvent(EntityId);