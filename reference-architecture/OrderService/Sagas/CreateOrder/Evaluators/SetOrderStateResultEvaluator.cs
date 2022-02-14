using EventDriven.Sagas.Abstractions.Evaluators;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Sagas.CreateOrder.Evaluators;

public class SetOrderStateResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult) =>
        Task.FromResult(commandResult == expectedResult);
}