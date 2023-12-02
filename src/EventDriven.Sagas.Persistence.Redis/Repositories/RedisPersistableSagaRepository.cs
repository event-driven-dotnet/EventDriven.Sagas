using System.Text.Json;
using AutoMapper;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using NeoSmart.AsyncLock;

namespace EventDriven.Sagas.Persistence.Redis.Repositories;

/// <summary>
/// Persistable saga repository.
/// </summary>
public class RedisPersistableSagaRepository<TSaga> : IPersistableSagaRepository<TSaga>
    where TSaga : PersistableSaga
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _cacheOptions;
    private readonly IMapper _mapper;
    private readonly AsyncLock _syncRoot = new();

    /// <summary>
    /// PersistableSagaRepository constructor.
    /// </summary>
    /// <param name="cache">Distributed cache.</param>
    /// <param name="cacheOptions">Distributed cache entry options.</param>
    /// <param name="mapper">Auto mapper.</param>
    public RedisPersistableSagaRepository(IDistributedCache cache,
        DistributedCacheEntryOptions cacheOptions,
        IMapper mapper)
    {
        _cache = cache;
        _cacheOptions = cacheOptions;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<TSaga?> GetAsync(Guid id, TSaga newEntity)
    {
        var json = await _cache.GetStringAsync(id.ToString());
        if (json is null) return null;
        var dto = JsonSerializer.Deserialize<PersistableSagaDto>(json);
        _mapper.Map(dto, newEntity);
        return newEntity;
    }

    /// <inheritdoc />
    public async Task<TSaga> CreateAsync(TSaga newEntity)
    {
        using (await _syncRoot.LockAsync())
        {
            newEntity.ETag = Guid.NewGuid().ToString();
            var dto = _mapper.Map<PersistableSagaDto>(newEntity);
            dto.Id = Guid.NewGuid();
            await InsertAsync(dto);
            _mapper.Map(dto, newEntity);
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task<TSaga> SaveAsync(TSaga existingEntity, TSaga newEntity)
    {
        using (await _syncRoot.LockAsync())
        {
            var existing = await GetAsync(existingEntity.Id, newEntity);
            if (existing is null)
                return await CreateAsync(newEntity);

            existingEntity.ETag = Guid.NewGuid().ToString();
            var dto = _mapper.Map<PersistableSagaDto>(existingEntity);
            dto.Id = Guid.NewGuid();
            await RemoveAsync(existingEntity.Id);
            await InsertAsync(dto);
            _mapper.Map(dto, newEntity);
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Guid id) =>
        await _cache.RemoveAsync(id.ToString());
    
    private async Task InsertAsync(PersistableSagaDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        await _cache.SetStringAsync(dto.SagaId.ToString(), json, _cacheOptions);
    }
}

/// <summary>
/// Persistable saga repository with metadata.
/// </summary>
/// <typeparam name="TSaga">Saga</typeparam>
/// <typeparam name="TMetaData">Metadata</typeparam>
public class RedisPersistableSagaRepository<TSaga, TMetaData> :
    RedisPersistableSagaRepository<TSaga>,
    IPersistableSagaRepository<TSaga, TMetaData>
    where TSaga : PersistableSaga<TMetaData>
    where TMetaData : class
{
    /// <inheritdoc />
    public RedisPersistableSagaRepository(IDistributedCache cache,
        DistributedCacheEntryOptions cacheOptions, IMapper mapper) :
        base(cache, cacheOptions, mapper)
    {
    }
}