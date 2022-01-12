using System;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFakes;

public class FakeCommandResultEvaluator : CommandResultEvaluator<string?, string?>
{
    public override Task<bool> EvaluateCommandResultAsync(string? commandResult, string? expectedResult)
        => Task.FromResult(string.Compare(commandResult, expectedResult, 
            StringComparison.OrdinalIgnoreCase) == 0);
}