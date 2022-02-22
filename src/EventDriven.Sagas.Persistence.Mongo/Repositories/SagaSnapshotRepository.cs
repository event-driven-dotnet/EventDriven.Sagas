using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.DTO;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using MongoDB.Driver;
using URF.Core.Mongo;

namespace EventDriven.Sagas.Persistence.Mongo.Repositories;

/// <summary>
/// Saga history repository.
/// </summary>
public class SagaSnapshotRepository : DocumentRepository<SagaSnapshotDto>, ISagaSnapshotRepository
{
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="collection">IMongoCollection.</param>
    /// <param name="mapper">Auto mapper.</param>
    public SagaSnapshotRepository(
        IMongoCollection<SagaSnapshotDto> collection,
        IMapper mapper) : base(collection)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task RetrieveAsync(Guid id, PersistableSaga entity)
    {
        var max = await GetMax(id);
        var dto = await FindOneAsync(e => e.Sequence == max);
        _mapper.Map(dto, entity);
    }

    /// <inheritdoc />
    public async Task PersistAsync(PersistableSaga entity)
    {
        var max = await GetMax(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        entity.Sequence = max + 1;
        var dto = _mapper.Map<SagaSnapshotDto>(entity);
        dto.Id = Guid.NewGuid();
        RemoveNonExecutedItems(dto);
        await InsertOneAsync(dto);
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
        var existing = await FindManyAsync(e => e.SagaId == id);
        var max = existing.Count == 0
            ? 0
            : await Queryable()
                .Where(e => e.SagaId == id)
                .MaxAsync(e => e.Sequence);
        return max;
    }
}