using EventDriven.Sagas.Abstractions.Commands;
using TestClient.DTO;

// ReSharper disable once CheckNamespace
namespace OrderService.Domain.OrderAggregate.Commands.SagaCommands;

public record SetOrderStateInitial : SagaCommand<OrderState, OrderState>;
