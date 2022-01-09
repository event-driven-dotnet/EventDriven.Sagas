using EventDriven.DependencyInjection.URF.Mongo;

namespace OrderService.Configuration;

public class SagaSnapshotDatabaseSettings : IMongoDbSettings
{
    public string CollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}