using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Pools;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReserveFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReserveFulfilled>
{
    private readonly ISagaPool<CreateOrderSaga> _sagaPool;
    private readonly ILogger<CustomerCreditReserveFulfilledEventHandler> _logger;

    public CustomerCreditReserveFulfilledEventHandler(
        ISagaPool<CreateOrderSaga> sagaPool,
        ILogger<CustomerCreditReserveFulfilledEventHandler> logger)
    {
        _sagaPool = sagaPool;
        _logger = logger;
    }

    public async Task DispatchCommandResultAsync(CustomerCreditReserveResponse commandResult, 
        bool compensating)
    {
        // Get saga from pool to handle command result
        var saga = _sagaPool[commandResult.CorrelationId];
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