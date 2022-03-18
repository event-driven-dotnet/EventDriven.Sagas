using Common.Integration.Events;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using InventoryService.Domain.InventoryAggregate;
using InventoryService.Domain.InventoryAggregate.Commands;

namespace InventoryService.Integration.Handlers;

public class ProductInventoryReleaseRequestedEventHandler :
    IntegrationEventHandler<ProductInventoryReleaseRequested>
{
    private readonly ICommandHandler<Inventory, ReleaseInventory> _commandHandler;
    private readonly ILogger<ProductInventoryReleaseRequestedEventHandler> _logger;

    public ProductInventoryReleaseRequestedEventHandler(
        ICommandHandler<Inventory, ReleaseInventory> commandHandler,
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
        await _commandHandler.Handle(command, CancellationToken.None);
    }
}