using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveInventoryStock(Order Order) : Command(Order.Id);