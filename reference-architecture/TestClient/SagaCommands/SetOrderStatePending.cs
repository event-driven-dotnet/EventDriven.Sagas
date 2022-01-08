using EventDriven.Sagas.Abstractions.Commands;
using TestClient.DTO;

// ReSharper disable once CheckNamespace
namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStatePending : SagaCommand<OrderState, OrderState>;
