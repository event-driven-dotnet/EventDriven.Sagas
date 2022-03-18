using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class ReleaseProductInventoryCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, ReleaseProductInventory, ProductInventoryReleaseResponse>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReleaseProductInventoryCommandHandler> _logger;

    public ReleaseProductInventoryCommandHandler(
        IEventBus eventBus,
        ILogger<ReleaseProductInventoryCommandHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public override async Task HandleCommandAsync(ReleaseProductInventory command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReleaseProductInventory));
        
        try
        {
            _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ReleaseProductInventory)}");
            var @event = new ProductInventoryReleaseRequested(
                new ProductInventoryReleaseRequest(command.InventoryId, command.AmountRequested));
            await _eventBus.PublishAsync(@event,
                null, "v1");
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            await DispatchCommandResultAsync(new ProductInventoryReleaseResponse(
                command.InventoryId, command.AmountRequested,
                0, false), true);
        }
    }
}