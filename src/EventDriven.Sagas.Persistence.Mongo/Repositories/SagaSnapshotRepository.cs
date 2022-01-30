using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using URF.Core.Abstractions;
using URF.Core.Mongo;

namespace EventDriven.Sagas.Persistence.Mongo.Repositories;

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
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task RetrieveSagaSnapshotAsync(Guid id, PersistableSaga entity)
    {
        var max = await GetMax(id);
        var dto = await _documentRepository.FindOneAsync(e => e.Sequence == max);
        _mapper.Map(dto, entity);
    }

    /// <inheritdoc />
    public async Task PersistSagaSnapshotAsync(PersistableSaga entity)
    {
        var max = await GetMax(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        entity.Sequence = max + 1;
        var dto = _mapper.Map<SagaSnapshotDto>(entity);
        dto.Id = Guid.NewGuid();
        RemoveNonExecutedItems(dto);
        await _documentRepository.InsertOneAsync(dto);
    }

    private void RemoveNonExecutedItems(SagaSnapshotDto dto)
    {
        for (var i = dto.Steps.Count - 1; i > -1; i--)
        {
            var step = dto.Steps[i];
            if (step is { CompensatingAction.State: ActionState.Initial })
                step.CompensatingAction = null;
            if (step is { Action.State: ActionState.Initial })
                step.Action = null;
            if (step?.CompensatingAction == null && step?.Action == null)
                dto.Steps.Remove(step);
        }
    }

    private async Task<int> GetMax(Guid id)
    {
        var existing = await _documentRepository
            .FindManyAsync(e => e.SagaId == id);
        var max = existing.Count == 0
            ? 0
            : await _documentRepository.Queryable()
                .Where(e => e.SagaId == id)
                .MaxAsync(e => e.Sequence);
        return max;
    }
}