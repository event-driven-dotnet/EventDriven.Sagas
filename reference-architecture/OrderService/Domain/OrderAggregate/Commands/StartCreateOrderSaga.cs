using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public record StartCreateOrderSaga(Order? Entity, OrderMetadata OrderMetadata) : Command<Order>(Entity);