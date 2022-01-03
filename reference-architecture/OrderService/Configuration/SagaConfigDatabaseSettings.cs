namespace OrderService.Configuration;

public class SagaConfigDatabaseSettings
{
    public string SagaConfigCollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}