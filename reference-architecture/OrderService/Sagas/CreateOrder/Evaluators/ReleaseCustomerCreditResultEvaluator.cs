using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace OrderService.Sagas.CreateOrder.Evaluators;

public class ReleaseCustomerCreditResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, CustomerCreditReleaseResponse, CustomerCreditReleaseResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(CustomerCreditReleaseResponse? commandResult,
        CustomerCreditReleaseResponse? expectedResult) =>
        Task.FromResult(commandResult?.Success == true);
}