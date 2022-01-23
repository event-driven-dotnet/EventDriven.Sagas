using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Integration.Events;
using Integration.Models;

namespace CustomerService.Domain.CustomerAggregate.Handlers;

public class CustomerCommandHandler :
    ICommandHandler<Customer, CreateCustomer>,
    ICommandHandler<Customer, UpdateCustomer>,
    ICommandHandler<Customer, RemoveCustomer>,
    ICommandHandler<Customer, ReserveCredit>
{
    private readonly ICustomerRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<CustomerCommandHandler> _logger;

    public CustomerCommandHandler(
        ICustomerRepository repository,
        IEventBus eventBus,
        ILogger<CustomerCommandHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Customer>> Handle(CreateCustomer command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
        var events = command.Entity.Process(command);
            
        // Apply events
        var domainEvent = events.OfType<CustomerCreated>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.Add(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }

    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateCustomer));
        var events = command.Entity.Process(command);
            
        // Apply events
        var domainEvent = events.OfType<CustomerUpdated>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.Update(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }

    public async Task<CommandResult<Customer>> Handle(RemoveCustomer command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveCustomer));
        var customer = await _repository.Get(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var events = customer.Process(command);
        
        // Apply events
        var domainEvent = events.OfType<CustomerRemoved>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        customer.Apply(domainEvent);
        
        // Persist entity
        await _repository.Remove(command.EntityId);
        return new CommandResult<Customer>(CommandOutcome.Accepted);
    }

    public async Task<CommandResult<Customer>> Handle(ReserveCredit command)
    {
        // Process command to determine if customer has sufficient credit
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReserveCredit));
        var customer = await _repository.Get(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var events = customer.Process(command);
        
        // Apply events to reserve credit
        var domainEvent = events.OfType<CreditReserved>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        customer.Apply(domainEvent);
        
        // Persist credit reservation
        var entity = await _repository.Update(customer);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
        await _eventBus.PublishAsync(new CustomerCreditReserveFulfilled(
            new CustomerCreditReserveResponse(customer.Id, command.AmountRequested, customer.CreditAvailable)),
            nameof(CustomerCreditReserveFulfilled), "v1");
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
}