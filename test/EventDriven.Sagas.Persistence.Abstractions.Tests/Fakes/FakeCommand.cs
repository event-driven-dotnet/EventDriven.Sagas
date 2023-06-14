using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Persistence.Abstractions.Tests.Fakes;

public record FakeCommand : SagaCommand<string, string>;