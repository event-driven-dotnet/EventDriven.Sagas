using EventDriven.DDD.Abstractions.Repositories;
using InventoryService.Domain.InventoryAggregate;
using URF.Core.Abstractions;

namespace InventoryService.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IDocumentRepository<Inventory> _documentRepository;

    public InventoryRepository(IDocumentRepository<Inventory> documentRepository)
    {
        _documentRepository = documentRepository;
    }
    
    public async Task<IEnumerable<Inventory>> GetAsync() =>
        await _documentRepository.FindManyAsync();

    public async Task<Inventory?> GetAsync(Guid id) =>
        await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<Inventory?> AddAsync(Inventory entity)
    {
        var existing = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
        if (existing != null) return null;
        entity.SequenceNumber = 1;
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.InsertOneAsync(entity);
    }

    public async Task<Inventory?> UpdateAsync(Inventory entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
            throw new ConcurrencyException();
        entity.SequenceNumber = existing.SequenceNumber + 1;
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<int> RemoveAsync(Guid id) => 
        await _documentRepository.DeleteOneAsync(e => e.Id == id);
}