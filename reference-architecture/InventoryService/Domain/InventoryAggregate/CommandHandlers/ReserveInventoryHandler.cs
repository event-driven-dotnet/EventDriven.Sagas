using Common.Integration.Events;
using Common.Integration.Models;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Domain.InventoryAggregate.Events;
using InventoryService.Repositories;

namespace InventoryService.Domain.InventoryAggregate.CommandHandlers;

public class ReserveInventoryHandler : ICommandHandler<Inventory, ReserveInventory>
{
    private readonly IInventoryRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ReserveInventoryHandler> _logger;

    public ReserveInventoryHandler(
        IInventoryRepository repository,
        IEventBus eventBus,
        ILogger<ReserveInventoryHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Inventory>> Handle(ReserveInventory command, CancellationToken cancellationToken)
    {
        // Process command to determine if inventory is sufficient
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
        var domainEvent = inventory.Process(command);

        // Return if inventory reservation unsuccessful
        if (domainEvent is not InventoryReserveSucceeded succeededEvent)
            return await PublishInventoryReservedResponse(inventory, command.AmountRequested, false);

        // Apply events to reserve inventory
        inventory.Apply(succeededEvent);

        Inventory? entity = null;
        CommandResult<Inventory> result;
        try
        {
            // Persist inventory reservation
            entity = await _repository.UpdateAsync(inventory);
            if (entity == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
            
            // Publish event
            result = await PublishInventoryReservedResponse(entity, command.AmountRequested, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var inventoryReleasedEvent = inventory.Process(
                    new ReleaseInventory(inventory.Id, command.AmountRequested));
                inventory.Apply(inventoryReleasedEvent);
                entity = await _repository.UpdateAsync(inventory);
                if (entity == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishInventoryReservedResponse(entity ?? inventory, command.AmountRequested, false);
        }

        return result;
    }
    
    private async Task<CommandResult<Inventory>> PublishInventoryReservedResponse(Inventory inventory, int amountRequested, bool success)
    {
        // Publish response to event bus
        _logger.LogInformation("Publishing event: {EventName}", $"v1.{nameof(ProductInventoryReserveFulfilled)}");
        try
        {
            var @event = new ProductInventoryReserveFulfilled(
                new ProductInventoryReserveResponse(inventory.Id, amountRequested,
                    inventory.AmountAvailable, success));
            await _eventBus.PublishAsync(@event,
                nameof(ProductInventoryReserveFulfilled), "v1");
            return new CommandResult<Inventory>(CommandOutcome.Accepted, inventory);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Inventory>(CommandOutcome.NotHandled);
        }
    }
}