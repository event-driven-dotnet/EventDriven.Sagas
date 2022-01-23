using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Handlers;
using Integration.Events;
using Integration.Models;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReleaseFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReleaseFulfilled>
{
    private readonly ILogger<CustomerCreditReleaseFulfilledEventHandler> _logger;

    public Type? SagaType { get; set; }
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    public CustomerCreditReleaseFulfilledEventHandler(
        ILogger<CustomerCreditReleaseFulfilledEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(CustomerCreditReleaseFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReleaseFulfilled)}");
        if (SagaCommandResultHandler is ISagaCommandResultHandler<CustomerCreditReleaseResponse> handler)
            await handler.HandleCommandResultAsync(@event.CustomerCreditReleaseResponse, false);
    }
}