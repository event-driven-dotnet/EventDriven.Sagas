using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReleaseFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReleaseFulfilled>,
    ISagaCommandResultDispatcher<CustomerCreditReleaseResponse>
{
    private readonly ILogger<CustomerCreditReleaseFulfilledEventHandler> _logger;

    public Type? SagaType { get; set; } = typeof(CreateOrderSaga);

    public async Task DispatchCommandResultAsync(CustomerCreditReleaseResponse commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<CustomerCreditReleaseResponse> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }

    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    public CustomerCreditReleaseFulfilledEventHandler(
        ILogger<CustomerCreditReleaseFulfilledEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(CustomerCreditReleaseFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReleaseFulfilled)}");
        await DispatchCommandResultAsync(new CustomerCreditReleaseResponse(
            @event.CustomerCreditReleaseResponse.CustomerId,
            @event.CustomerCreditReleaseResponse.CreditRequested,
            @event.CustomerCreditReleaseResponse.CreditRemaining,
            @event.CustomerCreditReleaseResponse.Success
        ), true);
    }
}