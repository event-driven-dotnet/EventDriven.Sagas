using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate;
using URF.Core.Abstractions;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDocumentRepository<Order> _documentRepository;

    public OrderRepository(
        IDocumentRepository<Order> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IEnumerable<Order>> GetOrders() =>
        await _documentRepository.FindManyAsync();

    public async Task<IEnumerable<Order>> GetCustomerOrders(Guid customerId) =>
        await _documentRepository.FindManyAsync(e => e.CustomerId == customerId);

    public async Task<Order?> GetOrderAsync(Guid id) =>
        await _documentRepository.FindOneAsync(e => e.Id == id);

    public async Task<Order?> AddOrderAsync(Order entity)
    {
        var existing = await GetOrderAsync(entity.Id);
        if (existing != null) throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.InsertOneAsync(entity);
    }

    public async Task<Order?> UpdateOrderAsync(Order entity)
    {
        var existing = await GetOrderAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<Order?> AddUpdateOrderAsync(Order entity)
    {
        Order? result;
        var existing = await GetOrderAsync(entity.Id);
        if (existing == null) result = await AddOrderAsync(entity);
        else
        {
            entity.ETag = existing.ETag;
            result = await UpdateOrderAsync(entity);
        }
        return result;
    }

    public async Task<int> RemoveOrder(Guid id) =>
        await _documentRepository.DeleteOneAsync(e => e.Id == id);

    public async Task<OrderState?> GetOrderStateAsync(Guid id)
    {
        var existing = await GetOrderAsync(id);
        return existing?.State;
    }

    public async Task<Order?> UpdateOrderStateAsync(Order entity, OrderState orderState)
    {
        var existing = await GetOrderAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
            throw new ConcurrencyException(entity.Id);
        entity.ETag = Guid.NewGuid().ToString();
        entity.State = orderState;
        return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }
}