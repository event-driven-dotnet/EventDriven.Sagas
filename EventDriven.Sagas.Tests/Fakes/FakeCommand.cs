using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeCommand : ISagaCommand<string, string>
{
    public string? Name { get; set; }

    public string ExpectedResult { get; set; } = null!;

    public string Result { get; set; } = null!;
}