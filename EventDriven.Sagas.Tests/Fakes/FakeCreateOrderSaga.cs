using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeCreateOrderSaga : Saga
{
    public Scenario Scenario { get; set; }

    public FakeCreateOrderSaga()
    {
        Steps.Add(
            new SagaStep
            {
                Sequence = 1,
                Action = new SagaAction
                {
                    Command = "SetOrderStatePending",
                    ExpectedResult = "Success"
                },
                CompensatingAction = new SagaAction
                {
                    Command = "SetOrderStateInitiated",
                    ExpectedResult = "Success"
                }
            });
        Steps.Add(
            new SagaStep
            {
                Sequence = 2,
                Action = new SagaAction
                {
                    Command = "ReserveCustomerCredit",
                    ExpectedResult = "Success"
                },
                CompensatingAction = new SagaAction
                {
                    Command = "ReleaseCustomerCredit",
                    ExpectedResult = "Success"
                }
            });
        Steps.Add(
            new SagaStep
            {
                Sequence = 3,
                Action = new SagaAction
                {
                    Command = "ReserveInventory",
                    ExpectedResult = "Success"
                },
                CompensatingAction = new SagaAction
                {
                    Command = "ReleaseInventory",
                    ExpectedResult = "Success"
                }
            });
        Steps.Add(
            new SagaStep
            {
                Sequence = 4,
                Action = new SagaAction
                {
                    Command = "SetOrderStateCreated",
                    ExpectedResult = "Success"
                },
                CompensatingAction = new SagaAction
                {
                    Command = "SetOrderStatePending",
                    ExpectedResult = "Success"
                }
            });
    }

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}

public enum Scenario
{
    Complete,
    Rollback,
    CancelOnStep2,
    FailOnStep3
}