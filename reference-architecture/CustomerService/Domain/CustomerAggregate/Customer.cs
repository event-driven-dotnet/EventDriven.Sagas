using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate;

public class Customer :
    Entity,
    ICommandProcessor<CreateCustomer, Customer, CustomerCreated>,
    IEventApplier<CustomerCreated>,
    ICommandProcessor<UpdateCustomer, Customer, CustomerUpdated>,
    IEventApplier<CustomerUpdated>,
    ICommandProcessor<RemoveCustomer, CustomerRemoved>,
    IEventApplier<CustomerRemoved>,
    ICommandProcessor<ReserveCredit, Customer, CreditReserved>,
    IEventApplier<CreditReserveSucceeded>,
    IEventApplier<CreditReserveFailed>,
    ICommandProcessor<ReleaseCredit, Customer, CreditReleased>,
    IEventApplier<CreditReleased>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public decimal CreditAvailable { get; set; }
    public Address ShippingAddress { get; set; } = null!;

    public CustomerCreated Process(CreateCustomer command) =>
        // To process command, return a domain event
        new(command.Entity);

    public void Apply(CustomerCreated domainEvent) =>
    // Set Id
    Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();

    public CustomerUpdated Process(UpdateCustomer command) =>
        // To process command, return a domain event
        new(command.Entity);

    public void Apply(CustomerUpdated domainEvent)
    {
        // Set ETag
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public CustomerRemoved Process(RemoveCustomer command) =>
        // To process command, return a domain event
        new(command.EntityId);

    public void Apply(CustomerRemoved domainEvent)
    {
        // Could mutate state here to implement a soft delete
    }

    public CreditReserved Process(ReserveCredit command)
    {
        // If customer has sufficient credit, return CreditReserveSucceeded event
        if (CreditAvailable >= command.AmountRequested)
            return new CreditReserveSucceeded(command.EntityId, command.AmountRequested) { EntityETag = ETag };
        // Otherwise, return CreditReserveFailed event
        return new CreditReserveFailed(command.EntityId, command.AmountRequested) { EntityETag = ETag };
    }

    public void Apply(CreditReserveSucceeded domainEvent)
    {
        CreditAvailable -= domainEvent.AmountRequested;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
    
    public void Apply(CreditReserveFailed domainEvent)
    {
        CreditAvailable += domainEvent.AmountRequested;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
    
    public CreditReleased Process(ReleaseCredit command) =>
        // To process command, return a domain event
        new(command.EntityId, command.AmountReleased) { EntityETag = ETag };

    public void Apply(CreditReleased domainEvent)
    {
        CreditAvailable += domainEvent.AmountRequested;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
}