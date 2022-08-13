using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Commands;

public record SetOrderStatePending(Guid? EntityId = default) : SagaCommand<OrderState, OrderState>(EntityId);
