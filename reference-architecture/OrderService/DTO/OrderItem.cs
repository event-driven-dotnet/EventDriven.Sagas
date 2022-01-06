namespace OrderService.DTO;

public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);