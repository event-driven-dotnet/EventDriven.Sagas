using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events;

public record CustomerUpdated(Customer Customer) : DomainEvent(Customer.Id);
