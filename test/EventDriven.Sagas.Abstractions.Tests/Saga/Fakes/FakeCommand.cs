using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public record FakeCommand : SagaCommand<string, string>;