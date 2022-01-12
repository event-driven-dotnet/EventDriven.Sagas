using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactoryFakes;

public record FakeSagaCommand : SagaCommand<string, string>;
