using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace SagaSnapshotService.Repositories;

public interface ISagaSnapshotDtoRepository
{
    Task<SagaSnapshotDto?> GetAsync(Guid id);
    Task<IEnumerable<SagaSnapshotDto>> GetSagaAsync(Guid sagaId);
    Task<int> RemoveSagaAsync(Guid sagaId);
}