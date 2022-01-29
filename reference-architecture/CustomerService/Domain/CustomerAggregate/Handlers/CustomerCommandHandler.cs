using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
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
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
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
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
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
        var domainEvent = customer.Process(command);
        
        // Apply events
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
        var domainEvent = customer.Process(command);

        // Return if credit reservation unsuccessful
        if (domainEvent is not CreditReserveSucceeded succeededEvent)
            return await PublishResponse(customer, command.CreditRequested, false);

        // Apply events to reserve credit
        customer.Apply(succeededEvent);

        Customer? entity = null;
        CommandResult<Customer> result;

        try
        {
            // Persist credit reservation
            entity = await _repository.Update(customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            result = await PublishResponse(entity, command.CreditRequested, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var creditReleasedEvent = customer.Process(
                    new ReleaseCredit(customer.Id, command.CreditRequested));
                customer.Apply(creditReleasedEvent);
                entity = await _repository.Update(customer);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishResponse(entity ?? customer, command.CreditRequested, false);
        }

        return result;
    }

    private async Task<CommandResult<Customer>> PublishResponse(Customer customer, decimal creditRequested, bool success)
    {
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
        try
        {
            await _eventBus.PublishAsync(new CustomerCreditReserveFulfilled(
                    new CustomerCreditReserveResponse(customer.Id, creditRequested,
                        customer.CreditAvailable, success)),
                nameof(CustomerCreditReserveFulfilled), "v1");
            return new CommandResult<Customer>(CommandOutcome.Accepted, customer);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Customer>(CommandOutcome.NotHandled);
        }
    }
}