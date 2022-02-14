using EventDriven.Sagas.Abstractions.Evaluators;
using Integration.Models;

namespace OrderService.Sagas.CreateOrder.Evaluators;

public class ReserveCustomerCreditResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, CustomerCreditReserveResponse, CustomerCreditReserveResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(CustomerCreditReserveResponse? commandResult,
        CustomerCreditReserveResponse? expectedResult) =>
        Task.FromResult(commandResult?.Success == true);
}