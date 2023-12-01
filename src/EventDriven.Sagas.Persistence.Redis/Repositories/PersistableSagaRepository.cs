using System.Text.Json;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using NeoSmart.AsyncLock;
using StackExchange.Redis;

namespace EventDriven.Sagas.Persistence.Redis.Repositories;

/// <summary>
/// Persistable saga repository.
/// </summary>
public class PersistableSagaRepository<TSaga> : IPersistableSagaRepository<TSaga>
    where TSaga : PersistableSaga
{
    private readonly IDatabaseAsync _db;
    private readonly AsyncLock _syncRoot = new();

    /// <summary>
    /// PersistableSagaRepository constructor.
    /// </summary>
    /// <param name="db">Redis database.</param>
    public PersistableSagaRepository(IDatabaseAsync db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<TSaga?> GetAsync(Guid id, TSaga newEntity)
    {
        using (await _syncRoot.LockAsync())
        {
            var json = await _db.StringGetAsync(id.ToString());
            if (json.IsNullOrEmpty) return null;
            newEntity = JsonSerializer.Deserialize<TSaga>(json.ToString())!;
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task<TSaga> CreateAsync(TSaga newEntity)
    {
        using (await _syncRoot.LockAsync())
        {
            newEntity.ETag = Guid.NewGuid().ToString();
            newEntity.Id = Guid.NewGuid();
            var json = JsonSerializer.Serialize(newEntity);
            await _db.StringSetAsync(newEntity.Id.ToString(), json, TimeSpan.FromMinutes(5));
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task<TSaga> SaveAsync(TSaga existingEntity, TSaga newEntity)
    {
        using (await _syncRoot.LockAsync())
        {
            var existing = await GetAsync(existingEntity.Id, newEntity);
            
            if (existing == null)
                return await CreateAsync(newEntity);

            newEntity.ETag = Guid.NewGuid().ToString();
            newEntity.Id = existingEntity.Id;
            var json = JsonSerializer.Serialize(newEntity);
            
            await _db.StringSetAsync(newEntity.Id.ToString(), json, TimeSpan.FromMinutes(5));
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Guid id) => await _db.KeyDeleteAsync(id.ToString());
}

/// <summary>
/// Persistable saga repository with metadata.
/// </summary>
/// <typeparam name="TSaga">Saga</typeparam>
/// <typeparam name="TMetaData">Metadata</typeparam>
public class PersistableSagaRepository<TSaga, TMetaData> :
    PersistableSagaRepository<TSaga>,
    IPersistableSagaRepository<TSaga, TMetaData>
    where TSaga : PersistableSaga<TMetaData>
    where TMetaData : class
{
    /// <inheritdoc />
    public PersistableSagaRepository(IDatabaseAsync db) : base(db)
    {
    }
}