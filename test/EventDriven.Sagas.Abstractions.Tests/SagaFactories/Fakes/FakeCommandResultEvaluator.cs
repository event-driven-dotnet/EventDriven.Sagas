using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactories.Fakes;

public class FakeCommandResultEvaluator : SagaCommandResultEvaluator<string, string>
{
    public override Task<bool> EvaluateCommandResultAsync(string? commandResult, string? expectedResult) => 
        Task.FromResult(commandResult == expectedResult);
}