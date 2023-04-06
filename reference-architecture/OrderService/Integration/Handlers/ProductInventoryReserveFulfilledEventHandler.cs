using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Pools;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class ProductInventoryReserveFulfilledEventHandler : 
    IntegrationEventHandler<ProductInventoryReserveFulfilled>
{
    private readonly ISagaPool<CreateOrderSaga> _sagaPool;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<ProductInventoryReserveFulfilledEventHandler> _logger;
    public Type? SagaType { get; set; } = typeof(CreateOrderSaga);

    public ProductInventoryReserveFulfilledEventHandler(
        ISagaPool<CreateOrderSaga> sagaPool,
        IOrderRepository orderRepository,
        ILogger<ProductInventoryReserveFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task DispatchCommandResultAsync(ProductInventoryReserveResponse commandResult, bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = await _sagaPool.GetSagaAsync(commandResult.CorrelationId,
            async entityId => await _orderRepository.GetAsync(entityId));
        await saga.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public override async Task HandleAsync(ProductInventoryReserveFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReserveFulfilled)}");
        
        await DispatchCommandResultAsync(new ProductInventoryReserveResponse(
            @event.ProductInventoryReserveResponse.InventoryId,
            @event.ProductInventoryReserveResponse.AmountRequested,
            @event.ProductInventoryReserveResponse.AmountAvailable,
            @event.ProductInventoryReserveResponse.Success,
            @event.ProductInventoryReserveResponse.CorrelationId
        ), false);
    }
}