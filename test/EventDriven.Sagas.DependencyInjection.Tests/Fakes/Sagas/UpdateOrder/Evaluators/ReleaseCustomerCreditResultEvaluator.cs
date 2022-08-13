using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Evaluators;

public class ReleaseCustomerCreditResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, CustomerCreditReserveResponse, CustomerCreditReserveResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(CustomerCreditReserveResponse? commandResult,
        CustomerCreditReserveResponse? expectedResult)
    {
        throw new NotImplementedException();
    }
}