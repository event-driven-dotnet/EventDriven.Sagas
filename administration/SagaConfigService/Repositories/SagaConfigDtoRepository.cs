using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using MongoDB.Driver;
using URF.Core.Mongo;

namespace SagaConfigService.Repositories;

public class SagaConfigDtoRepository : DocumentRepository<SagaConfigurationDto>, ISagaConfigDtoRepository
{
    public SagaConfigDtoRepository(
        IMongoCollection<SagaConfigurationDto> collection) : base(collection)
    {
    }

    public async Task<SagaConfigurationDto?> GetAsync(Guid id)
        => await FindOneAsync(e => e.Id == id);

    public async Task<SagaConfigurationDto?> AddAsync(SagaConfigurationDto entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await InsertOneAsync(entity);
    }

    public async Task<SagaConfigurationDto?> UpdateAsync(SagaConfigurationDto entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> RemoveAsync(Guid id) =>
        await DeleteOneAsync(e => e.Id == id);
}