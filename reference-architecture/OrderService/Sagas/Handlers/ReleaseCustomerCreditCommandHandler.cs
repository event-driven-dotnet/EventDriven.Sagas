using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Handlers;
using Integration.Events;
using Integration.Models;
using OrderService.Sagas.Commands;

namespace OrderService.Sagas.Handlers;

public class ReleaseCustomerCreditCommandHandler : ISagaCommandHandler<ReleaseCustomerCredit>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReleaseCustomerCreditCommandHandler> _logger;

    public ReleaseCustomerCreditCommandHandler(
        IEventBus eventBus,
        ILogger<ReleaseCustomerCreditCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleCommandAsync(ReleaseCustomerCredit command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReleaseCustomerCredit));
        
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ReleaseCustomerCredit)}");
        await _eventBus.PublishAsync(
            new CustomerCreditReleaseRequested(
                new CustomerCreditReleaseRequest(command.CustomerId, command.CreditReleased)),
            null, "v1");
    }
}