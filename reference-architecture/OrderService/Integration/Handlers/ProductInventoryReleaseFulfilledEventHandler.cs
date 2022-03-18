using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class ProductInventoryReleaseFulfilledEventHandler : 
    IntegrationEventHandler<ProductInventoryReleaseFulfilled>,
    ISagaCommandResultDispatcher<ProductInventoryReleaseResponse>
{
    private readonly ILogger<ProductInventoryReleaseFulfilledEventHandler> _logger;
    public Type? SagaType { get; set; } = typeof(CreateOrderSaga);
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    public async Task DispatchCommandResultAsync(ProductInventoryReleaseResponse commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<ProductInventoryReleaseResponse> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public ProductInventoryReleaseFulfilledEventHandler(
        ILogger<ProductInventoryReleaseFulfilledEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(ProductInventoryReleaseFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReleaseFulfilled)}");
        
        await DispatchCommandResultAsync(new ProductInventoryReleaseResponse(
            @event.ProductInventoryReleaseResponse.InventoryId,
            @event.ProductInventoryReleaseResponse.AmountRequested,
            @event.ProductInventoryReleaseResponse.AmountRemaining,
            @event.ProductInventoryReleaseResponse.Success
        ), true);
    }
}