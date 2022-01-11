using EventDriven.Sagas.Abstractions.Commands;

namespace SagaFactoryTest;

public class FakeCommandResultEvaluator : CommandResultEvaluator<string, string>
{
    public override Task<bool> EvaluateCommandResultAsync(string commandResult, string expectedResult)
    {
        var result = commandResult == expectedResult;
        return Task.FromResult(result);
    }
}