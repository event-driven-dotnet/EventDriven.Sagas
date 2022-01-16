using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate;

public class Customer :
    Entity,
    ICommandProcessor<CreateCustomer>,
    IEventApplier<CustomerCreated>,
    ICommandProcessor<UpdateCustomer>,
    IEventApplier<CustomerUpdated>,
    ICommandProcessor<RemoveCustomer>,
    IEventApplier<CustomerRemoved>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;

    public IEnumerable<IDomainEvent> Process(CreateCustomer command) =>
        // To process command, return one or more domain events
        new List<IDomainEvent>
        {
            new CustomerCreated(command.Customer)
        };

    public void Apply(CustomerCreated domainEvent) =>
    // Set Id
    Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();

    public IEnumerable<IDomainEvent> Process(RemoveCustomer command) =>
        // To process command, return one or more domain events
        new List<IDomainEvent>
        {
            new CustomerRemoved(command.EntityId)
        };

    public void Apply(CustomerRemoved domainEvent)
    {
        // Could mutate state here to implement a soft delete
    }

    public IEnumerable<IDomainEvent> Process(UpdateCustomer command) =>
    // To process command, return one or more domain events
    new List<IDomainEvent>
    {
        new CustomerUpdated(command.Customer)
    };

    public void Apply(CustomerUpdated domainEvent)
    {
        // Set ETag
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
}