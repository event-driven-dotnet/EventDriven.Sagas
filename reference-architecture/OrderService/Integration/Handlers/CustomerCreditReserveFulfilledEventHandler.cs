using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using Integration.Events;
using Integration.Models;
using OrderService.Sagas;

namespace OrderService.Integration.Handlers;

public class CustomerCreditReserveFulfilledEventHandler : 
    IntegrationEventHandler<CustomerCreditReserveFulfilled>,
    ISagaCommandResultDispatcher<CustomerCreditReserveResponse>
{
    private readonly ILogger<CustomerCreditReserveFulfilledEventHandler> _logger;
    public Type? SagaType { get; set; } = typeof(CreateOrderSaga);
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    public async Task DispatchCommandResultAsync(CustomerCreditReserveResponse commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<CustomerCreditReserveResponse> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
    
    public CustomerCreditReserveFulfilledEventHandler(
        ILogger<CustomerCreditReserveFulfilledEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(CustomerCreditReserveFulfilled @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
        await DispatchCommandResultAsync(new CustomerCreditReserveResponse(
            @event.CustomerCreditReserveResponse.CustomerId,
            @event.CustomerCreditReserveResponse.CreditReserved,
            @event.CustomerCreditReserveResponse.CreditRemaining
        ), false);
    }
}