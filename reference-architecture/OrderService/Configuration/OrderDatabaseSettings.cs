namespace OrderService.Configuration;

public class OrderDatabaseSettings
{
    public string OrderCollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}