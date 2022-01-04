using EventDriven.DependencyInjection.URF.Mongo;

namespace OrderService.Configuration;

public class SagaConfigDatabaseSettings : IMongoDbSettings
{
    public string CollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}