using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record ReserveCustomerCredit(Order Order) : Command(Order.Id);