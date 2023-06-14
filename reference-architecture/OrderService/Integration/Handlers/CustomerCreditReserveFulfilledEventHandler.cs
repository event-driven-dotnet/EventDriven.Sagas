using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Pools;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReserveFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReserveFulfilled>
{
    private readonly IPersistableSagaPool<CreateOrderSaga,OrderMetadata> _sagaPool;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CustomerCreditReserveFulfilledEventHandler> _logger;

    public CustomerCreditReserveFulfilledEventHandler(
        IPersistableSagaPool<CreateOrderSaga, OrderMetadata> sagaPool,
        IOrderRepository orderRepository,
        ILogger<CustomerCreditReserveFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task DispatchCommandResultAsync(CustomerCreditReserveResponse commandResult, 
        bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = await _sagaPool.GetSagaAsync(commandResult.CorrelationId,
            async entityId => await _orderRepository.GetAsync(entityId));
        await saga.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public override async Task HandleAsync(CustomerCreditReserveFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
        await DispatchCommandResultAsync(new CustomerCreditReserveResponse(
            @event.CustomerCreditReserveResponse.CustomerId,
            @event.CustomerCreditReserveResponse.CreditRequested,
            @event.CustomerCreditReserveResponse.CreditAvailable,
            @event.CustomerCreditReserveResponse.Success,
            @event.CustomerCreditReserveResponse.CorrelationId
        ), false);
    }
}