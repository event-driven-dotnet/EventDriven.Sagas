using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;
using InventoryService.Domain.InventoryAggregate.Commands;
using InventoryService.Domain.InventoryAggregate.Events;

namespace InventoryService.Domain.InventoryAggregate;

public class Inventory :
    Entity,
    ICommandProcessor<CreateInventory, Inventory, InventoryCreated>,
    IEventApplier<InventoryCreated>,
    ICommandProcessor<UpdateInventory, Inventory, InventoryUpdated>,
    IEventApplier<InventoryUpdated>,
    ICommandProcessor<RemoveInventory, InventoryRemoved>,
    IEventApplier<InventoryRemoved>,
    ICommandProcessor<ReserveInventory, Inventory, InventoryReserved>,
    IEventApplier<InventoryReserveSucceeded>,
    IEventApplier<InventoryReserveFailed>,
    ICommandProcessor<ReleaseInventory, Inventory, InventoryReleased>,
    IEventApplier<InventoryReleased>
{
    public string Description { get; set; } = null!;
    public int AmountAvailable { get; set; }

    public InventoryCreated Process(CreateInventory command) => new(command.Entity);

    public void Apply(InventoryCreated domainEvent) =>
        Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();

    public InventoryUpdated Process(UpdateInventory command) => new(command.Entity);

    public void Apply(InventoryUpdated domainEvent)
    {
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public InventoryRemoved Process(RemoveInventory command) => new(command.EntityId);

    public void Apply(InventoryRemoved domainEvent)
    {
    }

    public InventoryReserved Process(ReserveInventory command)
    {
        // If product has sufficient inventory, return InventoryReserveSucceeded event
        if (AmountAvailable >= command.AmountRequested)
            return new InventoryReserveSucceeded(command.EntityId, command.AmountRequested) { EntityETag = ETag };
        // Otherwise, return InventoryReserveFailed event
        return new InventoryReserveFailed(command.EntityId, command.AmountRequested) { EntityETag = ETag };
    }

    public void Apply(InventoryReserveSucceeded domainEvent)
    {
        AmountAvailable -= domainEvent.AmountRequested;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public void Apply(InventoryReserveFailed domainEvent)
    {
        AmountAvailable += domainEvent.AmountRequested;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }

    public InventoryReleased Process(ReleaseInventory command) =>
        new(command.EntityId, command.AmountReleased) { EntityETag = ETag };

    public void Apply(InventoryReleased domainEvent)
    {
        AmountAvailable += domainEvent.AmountReleased;
        if (domainEvent.EntityETag != null) ETag = domainEvent.EntityETag;
    }
}