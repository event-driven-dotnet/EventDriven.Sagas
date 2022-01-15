using System;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class FakeCommandResultEvaluator : SagaCommandResultEvaluator<string?, string?>
{
    public override Task<bool> EvaluateCommandResultAsync(string? commandResult, string? expectedResult)
        => Task.FromResult(string.Compare(commandResult, expectedResult, 
            StringComparison.OrdinalIgnoreCase) == 0);
}