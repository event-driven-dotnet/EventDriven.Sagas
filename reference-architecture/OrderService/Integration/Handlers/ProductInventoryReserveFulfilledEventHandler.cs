using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class ProductInventoryReserveFulfilledEventHandler : 
    IntegrationEventHandler<ProductInventoryReserveFulfilled>,
    ISagaCommandResultDispatcher<ProductInventoryReserveResponse>
{
    private readonly ILogger<ProductInventoryReserveFulfilledEventHandler> _logger;
    public Type? SagaType { get; set; } = typeof(CreateOrderSaga);
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    public async Task DispatchCommandResultAsync(ProductInventoryReserveResponse commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<ProductInventoryReserveResponse> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public ProductInventoryReserveFulfilledEventHandler(
        ILogger<ProductInventoryReserveFulfilledEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(ProductInventoryReserveFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReserveFulfilled)}");
        
        await DispatchCommandResultAsync(new ProductInventoryReserveResponse(
            @event.ProductInventoryReserveResponse.InventoryId,
            @event.ProductInventoryReserveResponse.AmountRequested,
            @event.ProductInventoryReserveResponse.AmountAvailable,
            @event.ProductInventoryReserveResponse.Success
        ), false);
    }
}