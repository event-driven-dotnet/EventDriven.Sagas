using EventDriven.DDD.Abstractions.Commands;
using EventDriven.EventBus.Abstractions;
using Integration.Events;
using InventoryService.Domain.InventoryAggregate.Commands;

namespace InventoryService.Integration.Handlers;

public class ProductInventoryReserveRequestedEventHandler :
    IntegrationEventHandler<ProductInventoryReserveRequested>
{
    private readonly ICommandHandler<ReserveInventory> _commandHandler;
    private readonly ILogger<ProductInventoryReserveRequestedEventHandler> _logger;

    public ProductInventoryReserveRequestedEventHandler(
        ICommandHandler<ReserveInventory> commandHandler,
        ILogger<ProductInventoryReserveRequestedEventHandler> logger)
    {
        _commandHandler = commandHandler;
        _logger = logger;
    }
    
    public override async Task HandleAsync(ProductInventoryReserveRequested @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(ProductInventoryReserveRequested)}");

        var command = new ReserveInventory(
            @event.ProductInventoryReserveRequest.InventoryId,
            @event.ProductInventoryReserveRequest.AmountReserved);
        await _commandHandler.Handle(command);
    }
}