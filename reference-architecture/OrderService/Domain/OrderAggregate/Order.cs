using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.DDD.Abstractions.Events;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Domain.OrderAggregate.Events;

namespace OrderService.Domain.OrderAggregate;

public class Order :
    Entity,
    ICommandProcessor<StartCreateOrderSaga, Order, CreateOrderSagaStarted>,
    IEventApplier<CreateOrderSagaStarted>
{
    public Guid CustomerId { get; set; }
    public Guid InventoryId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = null!;
    public OrderState State { get; set; }

    public CreateOrderSagaStarted Process(StartCreateOrderSaga command) =>
        new(command.EntityId);

    public void Apply(CreateOrderSagaStarted domainEvent) =>
        Id = domainEvent.EntityId != default(Guid) ? domainEvent.EntityId : Guid.NewGuid();
}