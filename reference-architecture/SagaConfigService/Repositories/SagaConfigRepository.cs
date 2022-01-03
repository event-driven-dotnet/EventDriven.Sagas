using SagaConfigService.DTO;
using URF.Core.Abstractions;

namespace SagaConfigService.Repositories;

public class SagaConfigRepository : ISagaConfigRepository
{
    private readonly IDocumentRepository<SagaConfiguration> _documentRepository;

    public SagaConfigRepository(
        IDocumentRepository<SagaConfiguration> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<SagaConfiguration?> GetSagaConfigurationAsync(Guid id)
        => await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<SagaConfiguration> AddSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existing = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
        if (existing != null) throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.InsertOneAsync(entity);
    }

    public async Task<SagaConfiguration> UpdateSagaConfigurationAsync(SagaConfiguration entity)
    {
        var existing = await GetSagaConfigurationAsync(entity.Id);
        if (existing == null || string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> RemoveSagaConfigurationAsync(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}