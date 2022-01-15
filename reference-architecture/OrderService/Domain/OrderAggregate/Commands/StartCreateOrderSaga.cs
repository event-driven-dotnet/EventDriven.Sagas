using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record StartCreateOrderSaga(Order Order) : Command(Order.Id);