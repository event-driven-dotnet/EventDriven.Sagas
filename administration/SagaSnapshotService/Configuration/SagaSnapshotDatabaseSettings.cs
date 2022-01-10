using EventDriven.DependencyInjection.URF.Mongo;

namespace SagaSnapshotService.Configuration;

public class SagaSnapshotDatabaseSettings : IMongoDbSettings
{
    public string CollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}