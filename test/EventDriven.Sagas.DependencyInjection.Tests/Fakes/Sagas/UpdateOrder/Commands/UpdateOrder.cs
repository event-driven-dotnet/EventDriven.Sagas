using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Commands;

public record UpdateOrder : SagaCommand<OrderState, OrderState>;
