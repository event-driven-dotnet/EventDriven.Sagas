using EventDriven.Sagas.Persistence.Abstractions.DTO;
using URF.Core.Abstractions;

namespace SagaSnapshotService.Repositories;

public class SagaSnapshotDtoRepository : ISagaSnapshotDtoRepository
{
    private readonly IDocumentRepository<SagaSnapshotDto> _documentRepository;

    public SagaSnapshotDtoRepository(
        IDocumentRepository<SagaSnapshotDto> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<SagaSnapshotDto?> GetSagaSnapshotAsync(Guid id) => 
        await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<IEnumerable<SagaSnapshotDto>> GetSagaSnapshotsAsync(Guid sagaId) => 
        await _documentRepository.FindManyAsync(e => e.SagaId == sagaId);
}