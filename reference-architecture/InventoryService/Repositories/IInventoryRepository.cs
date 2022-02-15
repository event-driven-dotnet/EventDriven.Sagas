using InventoryService.Domain.InventoryAggregate;

namespace InventoryService.Repositories;

public interface IInventoryRepository
{
    Task<IEnumerable<Inventory>> GetAsync();
    Task<Inventory?> GetAsync(Guid id);
    Task<Inventory?> AddAsync(Inventory entity);
    Task<Inventory?> UpdateAsync(Inventory entity);
    Task<int> RemoveAsync(Guid id);
}