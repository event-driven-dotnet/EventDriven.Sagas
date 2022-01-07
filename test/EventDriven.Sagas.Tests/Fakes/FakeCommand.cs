using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public record FakeCommand : SagaCommand<string, string>;