using EventDriven.CQRS.Abstractions.Commands;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Repositories;

namespace InventoryService.Domain.InventoryAggregate.CommandHandlers;

public class RemoveInventoryHandler : ICommandHandler<RemoveInventory>
{
    private readonly IInventoryRepository _repository;

    public RemoveInventoryHandler(
        IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(RemoveInventory command, CancellationToken cancellationToken)
    {
        // Process command
        var inventory = await _repository.GetAsync(command.EntityId);
        if (inventory == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
        var domainEvent = inventory.Process(command);
        
        // Apply events
        inventory.Apply(domainEvent);
        
        // Persist entity
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult<Inventory>(CommandOutcome.Accepted);
    }
}