using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactories.Fakes;

public record FakeSagaCommand : SagaCommand<string, string>;
