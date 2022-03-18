using EventDriven.CQRS.Abstractions.Commands;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Repositories;

namespace InventoryService.Domain.InventoryAggregate.CommandHandlers;

public class CreateInventoryHandler : ICommandHandler<Inventory, CreateInventory>
{
    private readonly IInventoryRepository _repository;

    public CreateInventoryHandler(
        IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult<Inventory>> Handle(CreateInventory command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Inventory>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.AddAsync(command.Entity);
        if (entity == null) return new CommandResult<Inventory>(CommandOutcome.NotFound);
        return new CommandResult<Inventory>(CommandOutcome.Accepted, entity);
    }
}