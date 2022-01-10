using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace SagaSnapshotService.Repositories;

public interface ISagaSnapshotDtoRepository
{
    Task<SagaSnapshotDto?> GetSagaSnapshotAsync(Guid id);
    Task<IEnumerable<SagaSnapshotDto>> GetSagaSnapshotsAsync(Guid sagaId);
}