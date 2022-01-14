using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.Evaluators;

public class SetOrderStateResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga,OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult)
        => Task.FromResult(commandResult == expectedResult);
}