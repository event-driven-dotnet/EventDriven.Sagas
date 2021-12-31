using EventDriven.DDD.Abstractions.Entities;

namespace OrderService.Domain.OrderAggregate;

public class Order : Entity
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = null!;
    public OrderState State { get; set; }
}