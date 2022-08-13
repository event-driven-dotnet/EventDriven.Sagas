using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Evaluators;

public class ReserveCustomerCreditResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, CustomerCreditReserveResponse, CustomerCreditReserveResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(CustomerCreditReserveResponse? commandResult,
        CustomerCreditReserveResponse? expectedResult)
    {
        throw new NotImplementedException();
    }
}