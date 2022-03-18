using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;

namespace CustomerService.Integration.Handlers;

public class CustomerCreditReleaseRequestedEventHandler :
    IntegrationEventHandler<CustomerCreditReleaseRequested>
{
    private readonly ICommandHandler<Customer, ReleaseCredit> _commandHandler;
    private readonly ILogger<CustomerCreditReleaseRequestedEventHandler> _logger;

    public CustomerCreditReleaseRequestedEventHandler(
        ICommandHandler<Customer, ReleaseCredit> commandHandler,
        ILogger<CustomerCreditReleaseRequestedEventHandler> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }
    
    public override async Task HandleAsync(CustomerCreditReleaseRequested @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReleaseRequested)}");

        var command = new ReleaseCredit(
            @event.CustomerCreditReleaseRequest.CustomerId,
            @event.CustomerCreditReleaseRequest.CreditReleased);
        await _commandHandler.Handle(command, CancellationToken.None);
    }
}