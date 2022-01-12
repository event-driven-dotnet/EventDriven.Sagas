using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFakes;

public record FakeCommand : SagaCommand<string, string>;