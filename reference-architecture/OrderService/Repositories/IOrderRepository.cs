using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id);
    Task<Order?> AddOAsync(Order entity);
    Task<Order?> UpdateAsync(Order entity);
    Task<Order?> AddUpdateAsync(Order entity);
    Task<int> RemoveAsync(Guid id);
    Task<OrderState?> GetOrderStateAsync(Guid id);
    Task<Order?> UpdateOrderStateAsync(Guid id, OrderState orderState);
}
