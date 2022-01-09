using AutoMapper;
using EventDriven.Sagas.Abstractions.DTO;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

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
        var max = await _documentRepository.Queryable().MaxAsync(e => e.Sequence);
        var dto = await _documentRepository.FindOneAsync(e => e.Sequence == max);
        if (dto == null) return null;
        var result = _mapper.Map<Saga>(dto);
        return result;
    }

    /// <inheritdoc />
    public async Task<Saga?> AddSagaAsync(Saga entity)
    {
        var max = await _documentRepository.Queryable().MaxAsync(e => e.Sequence);
        entity.ETag = Guid.NewGuid().ToString();
        var dto = _mapper.Map<SagaSnapshotDto>(entity);
        dto.Sequence = max + 1;
        dto = await _documentRepository.InsertOneAsync(dto);
        var result =  _mapper.Map<Saga>(dto);
        return result;
    }
}