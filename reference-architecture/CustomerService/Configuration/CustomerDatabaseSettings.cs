using EventDriven.DependencyInjection.URF.Mongo;

namespace CustomerService.Configuration;

public class CustomerDatabaseSettings : IMongoDbSettings
{
    public string CollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}