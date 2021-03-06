namespace CustomerService.DTO.Write;

public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public decimal CreditAvailable { get; set; }
    public Address ShippingAddress { get; set; } = null!;
    public string ETag { get; set; } = null!;
}