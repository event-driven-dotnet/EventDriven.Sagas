namespace OrderService.Domain.OrderAggregate;

public class OrderMetadata
{
    public VendorInfo VendorInfo { get; set; } = null!;
}

public class VendorInfo
{
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Country { get; set; } = null!;
}