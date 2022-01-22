using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record StartCreateOrderSaga(Order Entity) : Command<Order>(Entity);