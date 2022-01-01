using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order> GetOrder(Guid id);
    Task<Order> AddOrder(Order entity);
    Task<Order> UpdateOrder(Order entity);
    Task<int> RemoveOrder(Guid id);
    Task<Order> UpdateOrderState(Order entity, OrderState orderState);
}
