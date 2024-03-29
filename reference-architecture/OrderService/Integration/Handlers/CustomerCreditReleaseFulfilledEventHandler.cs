using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Persistence.Pool.Abstractions.Pools;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReleaseFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReleaseFulfilled>
{
    private readonly IPersistableSagaPool<CreateOrderSaga,OrderMetadata> _sagaPool;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CustomerCreditReleaseFulfilledEventHandler> _logger;

    public CustomerCreditReleaseFulfilledEventHandler(
        IPersistableSagaPool<CreateOrderSaga, OrderMetadata> sagaPool,
        IOrderRepository orderRepository,
        ILogger<CustomerCreditReleaseFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task DispatchCommandResultAsync(CustomerCreditReleaseResponse commandResult, bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = await _sagaPool.GetSagaAsync(commandResult.CorrelationId, 
            async entityId => await _orderRepository.GetAsync(entityId));
        await saga.HandleCommandResultAsync(commandResult, compensating);
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