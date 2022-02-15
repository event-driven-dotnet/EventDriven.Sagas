using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.EventBus.Abstractions;
using EventDriven.EventBus.Dapr;
using Integration.Events;
using Integration.Models;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Domain.InventoryAggregate.Events;
using InventoryService.Repositories;

namespace InventoryService.Domain.InventoryAggregate.Handlers;

public class InventoryCommandHandler :
    ICommandHandler<Inventory, CreateInventory>,
    ICommandHandler<Inventory, UpdateInventory>,
    ICommandHandler<Inventory, RemoveInventory>,
    ICommandHandler<ReserveInventory>,
    ICommandHandler<ReleaseInventory>
{
    private readonly IInventoryRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<InventoryCommandHandler> _logger;

    public InventoryCommandHandler(
        IInventoryRepository repository,
        IEventBus eventBus,
        ILogger<InventoryCommandHandler> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CommandResult<Inventory>> Handle(CreateInventory command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateInventory));
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        return new CommandResult<Inventory>(CommandOutcome.Accepted, entity);
    }

    public async Task<CommandResult<Inventory>> Handle(UpdateInventory command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(UpdateInventory));
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.UpdateAsync(command.Entity);
        if (entity == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        return new CommandResult<Inventory>(CommandOutcome.Accepted, entity);
    }

    public async Task<CommandResult<Inventory>> Handle(RemoveInventory command)
    {
        // Process command
        _logger.LogInformation("Handling command: {CommandName}", nameof(RemoveInventory));
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        var domainEvent = inventory.Process(command);
        
        // Apply events
        inventory.Apply(domainEvent);
        
        // Persist entity
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Inventory>(CommandOutcome.Accepted);
    }
    
    public async Task<CommandResult> Handle(ReserveInventory command)
    {
        // Process command to determine if inventory is sufficient
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReserveInventory));
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        var domainEvent = inventory.Process(command);

        // Return if inventory reservation unsuccessful
        if (domainEvent is not InventoryReserveSucceeded succeededEvent)
            return await PublishInventoryReservedResponse(inventory, command.AmountRequested, false);

        // Apply events to reserve inventory
        inventory.Apply(succeededEvent);

        Inventory? entity = null;
        CommandResult result;
        try
        {
            // Persist inventory reservation
            entity = await _repository.UpdateAsync(inventory);
            if (entity == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
            
            // Publish event
            result = await PublishInventoryReservedResponse(entity, command.AmountRequested, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var inventoryReleasedEvent = inventory.Process(
                    new ReleaseInventory(inventory.Id, command.AmountRequested));
                inventory.Apply(inventoryReleasedEvent);
                entity = await _repository.UpdateAsync(inventory);
                if (entity == null) return new CommandResult(CommandOutcome.InvalidCommand);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishInventoryReservedResponse(entity ?? inventory, command.AmountRequested, false);
        }

        return result;
    }

    public async Task<CommandResult> Handle(ReleaseInventory command)
    {
        // Process command to release inventory
        _logger.LogInformation("Handling command: {CommandName}", nameof(ReserveInventory));
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        var domainEvent = inventory.Process(command);
        
        // Apply events to release inventory
        inventory.Apply(domainEvent);
        
        Inventory? entity = null;
        CommandResult result;
        try
        {
            // Persist inventory reservation
            entity = await _repository.UpdateAsync(inventory);
            if (entity == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
            
            // Publish event
            result = await PublishInventoryReleasedResponse(entity, command.AmountReleased, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var inventoryReleasedEvent = inventory.Process(
                    new ReleaseInventory(inventory.Id, command.AmountReleased));
                inventory.Apply(inventoryReleasedEvent);
                entity = await _repository.UpdateAsync(inventory);
                if (entity == null) return new CommandResult(CommandOutcome.InvalidCommand);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishInventoryReleasedResponse(entity ?? inventory, command.AmountReleased, false);
        }

        return result;
    }
    
    private async Task<CommandResult> PublishInventoryReservedResponse(Inventory inventory, int amountRequested, bool success)
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
    
    private async Task<CommandResult> PublishInventoryReleasedResponse(Inventory inventory, decimal amountRequested, bool success)
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
            return new CommandResult(CommandOutcome.Accepted);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult(CommandOutcome.NotHandled);
        }
    }
}