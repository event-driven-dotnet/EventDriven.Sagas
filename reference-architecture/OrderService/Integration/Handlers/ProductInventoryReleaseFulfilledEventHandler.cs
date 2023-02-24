using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Pools;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class ProductInventoryReleaseFulfilledEventHandler : 
    IntegrationEventHandler<ProductInventoryReleaseFulfilled>
{
    private readonly ISagaPool<CreateOrderSaga> _sagaPool;
    private readonly ILogger<ProductInventoryReleaseFulfilledEventHandler> _logger;

    public async Task DispatchCommandResultAsync(ProductInventoryReleaseResponse commandResult, bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = _sagaPool[commandResult.CorrelationId];
        await saga.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public ProductInventoryReleaseFulfilledEventHandler(
        ISagaPool<CreateOrderSaga> sagaPool,
        ILogger<ProductInventoryReleaseFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _logger = logger;
    }

    public override async Task HandleAsync(ProductInventoryReleaseFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReleaseFulfilled)}");
        
        await DispatchCommandResultAsync(new ProductInventoryReleaseResponse(
            @event.ProductInventoryReleaseResponse.InventoryId,
            @event.ProductInventoryReleaseResponse.AmountRequested,
            @event.ProductInventoryReleaseResponse.AmountRemaining,
            @event.ProductInventoryReleaseResponse.Success,
            @event.ProductInventoryReleaseResponse.CorrelationId
        ), true);
    }
}