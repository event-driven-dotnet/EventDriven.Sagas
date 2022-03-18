using Common.Integration.Events;
using Common.Integration.Models;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class ReleaseCreditHandler : ICommandHandler<Customer, ReleaseCredit>
{
    private readonly ICustomerRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReleaseCreditHandler> _logger;

    public ReleaseCreditHandler(
        ICustomerRepository repository,
        IEventBus eventBus,
        ILogger<ReleaseCreditHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Customer>> Handle(ReleaseCredit command, CancellationToken cancellationToken)
    {
        // Process command to release credit
        var customer = await _repository.GetAsync(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
        var domainEvent = customer.Process(command);
        
        // Apply events to release credit
        customer.Apply(domainEvent);
        
        Customer? entity = null;
        CommandResult<Customer> result;
        try
        {
            // Persist credit reservation
            entity = await _repository.UpdateAsync(customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
            
            // Publish event
            result = await PublishCreditReleasedResponse(entity, command.AmountReleased, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var creditReleasedEvent = customer.Process(
                    new ReleaseCredit(customer.Id, command.AmountReleased));
                customer.Apply(creditReleasedEvent);
                entity = await _repository.UpdateAsync(customer);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishCreditReleasedResponse(entity ?? customer, command.AmountReleased, false);
        }

        return result;
    }
    
    private async Task<CommandResult<Customer>> PublishCreditReleasedResponse(Customer customer, decimal creditRequested, bool success)
    {
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(CustomerCreditReleaseFulfilled)}");
        try
        {
            var @event = new CustomerCreditReleaseFulfilled(
                new CustomerCreditReleaseResponse(customer.Id, creditRequested,
                    customer.CreditAvailable, success));
            await _eventBus.PublishAsync(@event,
                nameof(CustomerCreditReleaseFulfilled), "v1");
            return new CommandResult<Customer>(CommandOutcome.Accepted, customer);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Customer>(CommandOutcome.NotHandled, customer);
        }
    }
}