using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace OrderService.Sagas.CreateOrder.Evaluators;

public class ReserveProductInventoryResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, ProductInventoryReserveResponse, ProductInventoryReserveResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(ProductInventoryReserveResponse? commandResult,
        ProductInventoryReserveResponse? expectedResult) =>
        Task.FromResult(commandResult?.Success == true);
}