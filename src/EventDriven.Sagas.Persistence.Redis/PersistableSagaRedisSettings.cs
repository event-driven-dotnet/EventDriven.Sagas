using Microsoft.Extensions.Caching.Distributed;

namespace EventDriven.Sagas.Persistence.Redis;

/// <summary>
/// Persistable saga Redis settings.
/// </summary>
public class PersistableSagaRedisSettings
{
    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Instance name.
    /// </summary>
    public string InstanceName { get; set; } = null!;

    /// <summary>
    /// Distributed cache entry options.
    /// </summary>
    public DistributedCacheEntryOptions DistributedCacheEntryOptions { get; set; } = null!;
}