using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSaga :
    Abstractions.Saga,
    ISagaCommandResultHandler<FakeEntity>
{
    public const string SuccessState = "Success";
    public const string FailureState = "Failure";

    public FakeSaga(
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) : 
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }

    protected override Task<bool> CheckLock(Guid entityId) => Task.FromResult(false);

    protected override async Task ExecuteCurrentActionAsync() =>
        await SagaCommandDispatcher.DispatchCommandAsync(new FakeSagaCommand
        {
            Name = "Fake Saga Command",
            ExpectedResult = "Success"
        }, false);

    protected override Task ExecuteCurrentCompensatingActionAsync() => throw new NotImplementedException();

    public async Task HandleCommandResultAsync(FakeEntity result, bool compensating)
    {
        var evaluator = GetCommandResultEvaluatorByResultType<FakeSaga, string, string>();
        var success = evaluator != null
            && await evaluator.EvaluateCommandResultAsync(result.State, SuccessState);
        StateInfo = success ? SuccessState : FailureState;
        State = SagaState.Executed;
    }
}