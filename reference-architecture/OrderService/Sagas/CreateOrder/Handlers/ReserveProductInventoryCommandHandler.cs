using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class ReserveProductInventoryCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, ReserveProductInventory, ProductInventoryReserveResponse>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReserveProductInventoryCommandHandler> _logger;

    public ReserveProductInventoryCommandHandler(
        IEventBus eventBus,
        ILogger<ReserveProductInventoryCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public override async Task HandleCommandAsync(ReserveProductInventory command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReserveProductInventory));
        
        try
        {
            _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ReserveProductInventory)}");
            var @event = new ProductInventoryReserveRequested(
                new ProductInventoryReserveRequest(command.InventoryId, command.AmountRequested));
            await _eventBus.PublishAsync(@event,
                nameof(ProductInventoryReserveRequested), "v1");
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            await DispatchCommandResultAsync(new ProductInventoryReserveResponse(
                command.InventoryId, command.AmountRequested,
                0, false), true);
        }
    }
}