using Common.Integration.Events;
using Common.Integration.Models;
using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class ReserveCreditHandler : ICommandHandler<Customer, ReserveCredit>
{
    private readonly ICustomerRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReserveCreditHandler> _logger;

    public ReserveCreditHandler(
        ICustomerRepository repository,
        IEventBus eventBus,
        ILogger<ReserveCreditHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Customer>> Handle(ReserveCredit command, CancellationToken cancellationToken)
    {
        // Process command to determine if customer has sufficient credit
        var customer = await _repository.GetAsync(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
        var domainEvent = customer.Process(command);

        // Return if credit reservation unsuccessful
        if (domainEvent is not CreditReserveSucceeded succeededEvent)
            return await PublishCreditReservedResponse(customer, command.AmountRequested, false);

        // Apply events to reserve credit
        customer.Apply(succeededEvent);

        Customer? entity = null;
        CommandResult<Customer> result;
        try
        {
            // Persist credit reservation
            entity = await _repository.UpdateAsync(customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
            
            // Publish event
            result = await PublishCreditReservedResponse(entity, command.AmountRequested, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var creditReleasedEvent = customer.Process(
                    new ReleaseCredit(customer.Id, command.AmountRequested));
                customer.Apply(creditReleasedEvent);
                entity = await _repository.UpdateAsync(customer);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.NotFound);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishCreditReservedResponse(entity ?? customer, command.AmountRequested, false);
        }

        return result;
    }
    
    private async Task<CommandResult<Customer>> PublishCreditReservedResponse(Customer customer, decimal creditRequested, bool success)
    {
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
        try
        {
            var @event = new CustomerCreditReserveFulfilled(
                new CustomerCreditReserveResponse(customer.Id, creditRequested,
                    customer.CreditAvailable, success));
            await _eventBus.PublishAsync(@event,
                nameof(CustomerCreditReserveFulfilled), "v1");
            return new CommandResult<Customer>(CommandOutcome.Accepted, customer);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Customer>(CommandOutcome.NotHandled, customer);
        }
    }}