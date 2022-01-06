namespace OrderService.DTO;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = null!;
    public OrderState State { get; set; }
    public string ETag { get; set; } = null!;
}