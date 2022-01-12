using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public record FakeSagaCommand : SagaCommand<string, string>;
