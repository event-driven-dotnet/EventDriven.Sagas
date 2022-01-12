using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactoryFakes;

public class FakeCommandResultEvaluator : CommandResultEvaluator<string, string>
{
    public override Task<bool> EvaluateCommandResultAsync(string commandResult, string expectedResult) => 
        Task.FromResult(commandResult == expectedResult);
}