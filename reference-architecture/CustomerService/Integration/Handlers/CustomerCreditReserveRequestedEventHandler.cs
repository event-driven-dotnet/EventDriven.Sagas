using Common.Integration.Events;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;

namespace CustomerService.Integration.Handlers;

public class CustomerCreditReserveRequestedEventHandler :
    IntegrationEventHandler<CustomerCreditReserveRequested>
{
    private readonly ICommandHandler<Customer, ReserveCredit> _commandHandler;
    private readonly ILogger<CustomerCreditReserveRequestedEventHandler> _logger;

    public Guid Id { get; set; } = Guid.NewGuid();

    public CustomerCreditReserveRequestedEventHandler(
        ICommandHandler<Customer, ReserveCredit> commandHandler,
        ILogger<CustomerCreditReserveRequestedEventHandler> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }
    
    public override async Task HandleAsync(CustomerCreditReserveRequested @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReserveRequested)}");

        var command = new ReserveCredit(
            @event.CustomerCreditReserveRequest.CustomerId,
            @event.CustomerCreditReserveRequest.CreditReserved);
        await _commandHandler.Handle(command, CancellationToken.None);
    }
}