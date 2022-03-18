using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Evaluators;

namespace OrderService.Sagas.CreateOrder.Evaluators;

public class ReleaseProductInventoryResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, ProductInventoryReleaseResponse, ProductInventoryReleaseResponse>
{
    public override Task<bool> EvaluateCommandResultAsync(ProductInventoryReleaseResponse? commandResult,
        ProductInventoryReleaseResponse? expectedResult) =>
        Task.FromResult(commandResult?.Success == true);
}