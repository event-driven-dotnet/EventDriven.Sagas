using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using MongoDB.Driver;
using NeoSmart.AsyncLock;
using URF.Core.Mongo;

namespace EventDriven.Sagas.Persistence.Mongo.Repositories;

/// <summary>
/// Persistable saga repository.
/// </summary>
public class PersistableSagaRepository<TSaga> : 
    DocumentRepository<PersistableSagaDto>, IPersistableSagaRepository<TSaga>
    where TSaga : Saga
{
    private readonly IMapper _mapper;
    private readonly AsyncLock _syncRoot = new();

    /// <summary>
    /// PersistableSagaRepository constructor.
    /// </summary>
    /// <param name="collection">IMongoCollection.</param>
    /// <param name="mapper">Auto mapper.</param>
    public PersistableSagaRepository(
        IMongoCollection<PersistableSagaDto> collection,
        IMapper mapper) : base(collection)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<TSaga?> GetAsync(Guid id, TSaga newEntity)
    {
        var dto = await FindOneAsync(e => e.SagaId == id);
        if (dto is null) return null;
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
            await InsertOneAsync(dto);
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
            await InsertOneAsync(dto);
            _mapper.Map(dto, newEntity);
            return newEntity;
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(Guid id) => 
        await DeleteOneAsync(e => e.SagaId == id);
}