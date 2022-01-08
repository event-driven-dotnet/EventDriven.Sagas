using EventDriven.Sagas.Abstractions.Commands;

// ReSharper disable once CheckNamespace
namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record ReserveInventoryStock : SagaCommand;
