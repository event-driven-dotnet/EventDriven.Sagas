using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Handlers;
using Integration.Events;
using Integration.Models;
using OrderService.Sagas.Commands;

namespace OrderService.Sagas.Handlers;

public class ReserveCustomerCreditCommandHandler : ISagaCommandHandler<ReserveCustomerCredit>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReserveCustomerCreditCommandHandler> _logger;

    public ReserveCustomerCreditCommandHandler(
        IEventBus eventBus,
        ILogger<ReserveCustomerCreditCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleCommandAsync(ReserveCustomerCredit command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReserveCustomerCredit));
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ReserveCustomerCredit)}");
        await _eventBus.PublishAsync(
            new CustomerCreditReserveRequested(
                new CustomerCreditReserveRequest(command.CustomerId, command.CreditRequested)),
            nameof(CustomerCreditReserveRequested), "v1");
    }
}