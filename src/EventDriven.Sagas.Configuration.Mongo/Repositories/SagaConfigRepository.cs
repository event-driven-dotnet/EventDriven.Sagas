using AutoMapper;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using URF.Core.Abstractions;

namespace EventDriven.Sagas.Configuration.Mongo.Repositories;

/// <summary>
/// Saga configuration repository.
/// </summary>
public class SagaConfigRepository : ISagaConfigRepository
{
    private readonly IMapper _mapper;
    private readonly IDocumentRepository<SagaConfigurationDto> _documentRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="documentRepository">Document repository.</param>
    /// <param name="mapper">Auto mapper.</param>
    public SagaConfigRepository(
        IDocumentRepository<SagaConfigurationDto> documentRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> GetSagaConfigurationAsync(Guid id)
    {
        var dto = await _documentRepository.FindOneAsync(e => e.Id == id);
        if (dto == null) return null;
        var result = _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> AddSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existingDto = await GetSagaConfigurationAsync(entity.Id);
        if (existingDto != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await _documentRepository.InsertOneAsync(dto);
        var result =  _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<SagaConfiguration?> UpdateSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existingDto = await GetSagaConfigurationAsync(entity.Id);
        if (existingDto == null) return null;
        if (string.Compare(entity.ETag, existingDto.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaConfigurationDto>(entity);
        dto = await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, dto);
        var result =   _mapper.Map<SagaConfiguration>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<int> RemoveSagaConfigurationAsync(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}