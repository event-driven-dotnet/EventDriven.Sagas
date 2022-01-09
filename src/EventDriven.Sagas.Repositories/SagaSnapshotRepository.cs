using AutoMapper;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.DTO;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Repositories;
using URF.Core.Abstractions;

namespace EventDriven.Sagas.Repositories;

/// <summary>
/// Saga history repository.
/// </summary>
public class SagaSnapshotRepository : ISagaSnapshotRepository
{
    private readonly IMapper _mapper;
    private readonly IDocumentRepository<SagaSnapshotDto> _documentRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="documentRepository">Document repository.</param>
    /// <param name="mapper">Auto mapper.</param>
    public SagaSnapshotRepository(
        IDocumentRepository<SagaSnapshotDto> documentRepository,
        IMapper mapper)
    {
        _documentRepository = documentRepository;
        // TODO: Set up saga / saga snapshot mappings
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Saga?> GetSagaAsync(Guid id)
    {
        // TODO: Get the latest saga snapshot
        var dto = await _documentRepository.FindOneAsync(e => e.SagaId == id);
        if (dto == null) return null;
        var result = _mapper.Map<Saga>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<Saga?> AddSagaAsync(Saga entity)
    {
        var existingDto = await GetSagaAsync(entity.Id);
        if (existingDto != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaSnapshotDto>(entity);
        dto = await _documentRepository.InsertOneAsync(dto);
        var result =  _mapper.Map<Saga>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<Saga?> UpdateSagaAsync(Saga entity)
    {
        var existingDto = await GetSagaAsync(entity.Id);
        if (existingDto == null) return null;
        if (string.Compare(entity.ETag, existingDto.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaSnapshotDto>(entity);
        dto = await _documentRepository.FindOneAndReplaceAsync(e => e.SagaId == entity.Id, dto);
        var result =   _mapper.Map<Saga>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<int> RemoveSagaAsync(Guid id) => 
        await _documentRepository.DeleteOneAsync(e => e.SnapshotId == id);
}