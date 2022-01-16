using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Handlers;

public class CustomerCommandHandler :
    ICommandHandler<Customer, CreateCustomer>,
    ICommandHandler<Customer, UpdateCustomer>,
    ICommandHandler<Customer, RemoveCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerCommandHandler> _logger;

    public CustomerCommandHandler(
        ICustomerRepository repository,
        ILogger<CustomerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CommandResult<Customer>> Handle(CreateCustomer command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateCustomer));
        var events = command.Customer.Process(command);
            
        // Apply events
        var domainEvent = events.OfType<CustomerCreated>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        command.Customer.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.Add(command.Customer);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }

    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateCustomer));
        var events = command.Customer.Process(command);
            
        // Apply events
        var domainEvent = events.OfType<CustomerUpdated>().SingleOrDefault();
        if (domainEvent == null) return new CommandResult<Customer>(CommandOutcome.NotHandled);
        command.Customer.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.Update(command.Customer);
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
}