using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Pools;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReleaseFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReleaseFulfilled>
{
    private readonly ISagaPool<CreateOrderSaga> _sagaPool;
    private readonly ILogger<CustomerCreditReleaseFulfilledEventHandler> _logger;

    public async Task DispatchCommandResultAsync(CustomerCreditReleaseResponse commandResult, bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = _sagaPool[commandResult.CorrelationId];
        await saga.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public CustomerCreditReleaseFulfilledEventHandler(
        ISagaPool<CreateOrderSaga> sagaPool,
        ILogger<CustomerCreditReleaseFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _logger = logger;
    }

    public override async Task HandleAsync(CustomerCreditReleaseFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReleaseFulfilled)}");
        await DispatchCommandResultAsync(new CustomerCreditReleaseResponse(
            @event.CustomerCreditReleaseResponse.CustomerId,
            @event.CustomerCreditReleaseResponse.CreditRequested,
            @event.CustomerCreditReleaseResponse.CreditRemaining,
            @event.CustomerCreditReleaseResponse.Success,
            @event.CustomerCreditReleaseResponse.CorrelationId
        ), true);
    }
}