using System;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Configuration.Abstractions;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeConfigurableSaga : ConfigurableSaga<FakeEntity>
{
    public const string SuccessState = "Success";
    public const string FailureState = "Failure";

    public FakeConfigurableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        ISagaCommandResultEvaluator commandResultEvaluator) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
    }

    protected override async Task ExecuteCurrentActionAsync() =>
        await SagaCommandDispatcher.DispatchAsync(new FakeSagaCommand
        {
            Name = "Fake Saga Command",
            ExpectedResult = "Success"
        }, false);

    protected override Task ExecuteCurrentCompensatingActionAsync() => throw new NotImplementedException();
    public override async Task ProcessCommandResultAsync(FakeEntity commandResult, bool compensating)
    {
        var evaluator = (ISagaCommandResultEvaluator<string, string>)CommandResultEvaluator;
        var success = await evaluator.EvaluateCommandResultAsync(commandResult.State, SuccessState);
        StateInfo = success ? SuccessState : FailureState;
        State = SagaState.Executed;
    }
}