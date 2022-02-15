using EventDriven.DependencyInjection.URF.Mongo;

namespace InventoryService.Configuration;

public class InventoryDatabaseSettings : IMongoDbSettings
{
    public string CollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}