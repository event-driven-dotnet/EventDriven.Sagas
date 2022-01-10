using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Configuration.Abstractions.DTO;
using URF.Core.Abstractions;

namespace SagaConfigService.Repositories;

public class SagaConfigDtoRepository : ISagaConfigDtoRepository
{
    private readonly IDocumentRepository<SagaConfigurationDto> _documentRepository;

    public SagaConfigDtoRepository(
        IDocumentRepository<SagaConfigurationDto> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<SagaConfigurationDto?> GetSagaConfigurationAsync(Guid id)
        => await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<SagaConfigurationDto?> AddSagaConfigurationAsync(SagaConfigurationDto entity)
    {
        var existing = await GetSagaConfigurationAsync(entity.Id);
        if (existing != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.InsertOneAsync(entity);
    }

    public async Task<SagaConfigurationDto?> UpdateSagaConfigurationAsync(SagaConfigurationDto entity)
    {
        var existing = await GetSagaConfigurationAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> RemoveSagaConfigurationAsync(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}