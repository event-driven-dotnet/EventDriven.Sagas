using EventDriven.DDD.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Integration.Events;
using InventoryService.Domain.InventoryAggregate.Commands;

namespace InventoryService.Integration.Handlers;

public class ProductInventoryReleaseRequestedEventHandler :
    IntegrationEventHandler<ProductInventoryReleaseRequested>
{
    private readonly ICommandHandler<ReleaseInventory> _commandHandler;
    private readonly ILogger<ProductInventoryReleaseRequestedEventHandler> _logger;

    public ProductInventoryReleaseRequestedEventHandler(
        ICommandHandler<ReleaseInventory> commandHandler,
        ILogger<ProductInventoryReleaseRequestedEventHandler> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }
    
    public override async Task HandleAsync(ProductInventoryReleaseRequested @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReleaseRequested)}");

        var command = new ReleaseInventory(
            @event.ProductInventoryReleaseRequests.InventoryId,
            @event.ProductInventoryReleaseRequests.AmountReleased);
        await _commandHandler.Handle(command);
    }
}