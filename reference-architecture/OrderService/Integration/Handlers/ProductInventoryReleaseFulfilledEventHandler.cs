using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Persistence.Pool.Abstractions.Pools;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class ProductInventoryReleaseFulfilledEventHandler : 
    IntegrationEventHandler<ProductInventoryReleaseFulfilled>
{
    private readonly IPersistableSagaPool<CreateOrderSaga, OrderMetadata> _sagaPool;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<ProductInventoryReleaseFulfilledEventHandler> _logger;

    public ProductInventoryReleaseFulfilledEventHandler(
        IPersistableSagaPool<CreateOrderSaga,OrderMetadata> sagaPool,
        IOrderRepository orderRepository,
        ILogger<ProductInventoryReleaseFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task DispatchCommandResultAsync(ProductInventoryReleaseResponse commandResult, bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = await _sagaPool.GetSagaAsync(commandResult.CorrelationId,
            async entityId => await _orderRepository.GetAsync(entityId));
        await saga.HandleCommandResultAsync(commandResult, compensating);
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