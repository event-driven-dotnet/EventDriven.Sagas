using EventDriven.Sagas.Persistence.Abstractions.DTO;
using MongoDB.Driver;
using URF.Core.Mongo;

namespace SagaSnapshotService.Repositories;

public class SagaSnapshotDtoRepository : DocumentRepository<SagaSnapshotDto>, ISagaSnapshotDtoRepository
{
    public SagaSnapshotDtoRepository(
        IMongoCollection<SagaSnapshotDto> collection) : base(collection)
    {
    }

    public async Task<SagaSnapshotDto?> GetAsync(Guid id) => 
        await FindOneAsync(e => e.Id == id);

    public async Task<IEnumerable<SagaSnapshotDto>> GetSagaAsync(Guid sagaId) => 
        await FindManyAsync(e => e.SagaId == sagaId);

    public async Task<int> RemoveSagaAsync(Guid sagaId) =>
        await DeleteManyAsync(e => e.SagaId == sagaId);
}