using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReleased(Guid EntityId, decimal AmountRequested) : DomainEvent(EntityId);