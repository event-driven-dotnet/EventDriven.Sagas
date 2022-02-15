using CustomerService.Domain.CustomerAggregate.Commands;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Integration.Events;

namespace CustomerService.Integration.Handlers;

public class CustomerCreditReserveReleaseEventHandler :
    IntegrationEventHandler<CustomerCreditReleaseRequested>
{
    private readonly ICommandHandler<ReleaseCredit> _commandHandler;
    private readonly ILogger<CustomerCreditReserveReleaseEventHandler> _logger;

    public CustomerCreditReserveReleaseEventHandler(
        ICommandHandler<ReleaseCredit> commandHandler,
        ILogger<CustomerCreditReserveReleaseEventHandler> logger)
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
        await _commandHandler.Handle(command);
    }
}