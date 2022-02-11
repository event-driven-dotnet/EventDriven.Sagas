using OrderService.Domain.OrderAggregate;

namespace OrderService.Sagas.Specs.Helpers;

public class OrderComparer : IEqualityComparer<Order>
{
    public bool Equals(Order? x, Order? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode(Order obj) => HashCode.Combine(obj.Id);
}