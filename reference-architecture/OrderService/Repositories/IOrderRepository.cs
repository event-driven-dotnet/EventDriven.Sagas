using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetOrderAsync(Guid id);
    Task<Order?> AddOrderAsync(Order entity);
    Task<Order?> UpdateOrderAsync(Order entity);
    Task<Order?> AddUpdateOrderAsync(Order entity);
    Task<int> RemoveOrderAsync(Guid id);
    Task<OrderState?> GetOrderStateAsync(Guid id);
    Task<Order?> UpdateOrderStateAsync(Guid id, OrderState orderState);
}
