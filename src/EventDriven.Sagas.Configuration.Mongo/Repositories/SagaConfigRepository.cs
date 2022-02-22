using AutoMapper;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using MongoDB.Driver;
using URF.Core.Mongo;

namespace EventDriven.Sagas.Configuration.Mongo.Repositories;

/// <summary>
/// Saga configuration repository.
/// </summary>
public class SagaConfigRepository : DocumentRepository<SagaConfigurationDto>, ISagaConfigRepository
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="collection">IMongoCollection.</param>
    /// <param name="mapper">Auto mapper.</param>
    public SagaConfigRepository(
        IMongoCollection<SagaConfigurationDto> collection,
        IMapper mapper) : base(collection)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> GetAsync(Guid id)
    {
        var dto = await FindOneAsync(e => e.Id == id);
        if (dto == null) return null;
        var result = _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> AddAsync(SagaConfiguration entity)
    {
        var existingDto = await GetAsync(entity.Id);
        if (existingDto != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await InsertOneAsync(dto);
        var result =  _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> UpdateAsync(SagaConfiguration entity)
    {
        var existingDto = await GetAsync(entity.Id);
        if (existingDto == null) return null;
        if (string.Compare(entity.ETag, existingDto.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await FindOneAndReplaceAsync(e => e.Id == entity.Id, dto);
        var result =   _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<int> RemoveAsync(Guid id) =>
        await DeleteOneAsync(e => e.Id == id);
}