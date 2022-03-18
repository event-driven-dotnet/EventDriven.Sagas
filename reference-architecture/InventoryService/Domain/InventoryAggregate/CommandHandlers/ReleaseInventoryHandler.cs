using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Repositories;

namespace InventoryService.Domain.InventoryAggregate.CommandHandlers;

public class ReleaseInventoryHandler : ICommandHandler<Inventory, ReleaseInventory>
{
    private readonly IInventoryRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReleaseInventoryHandler> _logger;

    public ReleaseInventoryHandler(
        IInventoryRepository repository,
        IEventBus eventBus,
        ILogger<ReleaseInventoryHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Inventory>> Handle(ReleaseInventory command, CancellationToken cancellationToken)
    {
        // Process command to release inventory
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
        var domainEvent = inventory.Process(command);
        
        // Apply events to release inventory
        inventory.Apply(domainEvent);
        
        Inventory? entity = null;
        CommandResult<Inventory> result;
        try
        {
            // Persist inventory reservation
            entity = await _repository.UpdateAsync(inventory);
            if (entity == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
            
            // Publish event
            result = await PublishInventoryReleasedResponse(entity, command.AmountReleased, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var inventoryReleasedEvent = inventory.Process(
                    new ReleaseInventory(inventory.Id, command.AmountReleased));
                inventory.Apply(inventoryReleasedEvent);
                entity = await _repository.UpdateAsync(inventory);
                if (entity == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishInventoryReleasedResponse(entity ?? inventory, command.AmountReleased, false);
        }

        return result;
    }

    private async Task<CommandResult<Inventory>> PublishInventoryReleasedResponse(Inventory inventory, decimal amountRequested, bool success)
    {
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ProductInventoryReleaseFulfilled)}");
        try
        {
            var @event = new ProductInventoryReleaseFulfilled(
                new ProductInventoryReleaseResponse(inventory.Id, amountRequested,
                    inventory.AmountAvailable, success));
            await _eventBus.PublishAsync(@event,
                nameof(ProductInventoryReleaseFulfilled), "v1");
            return new CommandResult<Inventory>(CommandOutcome.Accepted, inventory);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Inventory>(CommandOutcome.NotHandled, inventory);
        }
    }
}