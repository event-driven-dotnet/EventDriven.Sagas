using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas;

namespace OrderService.Domain.OrderAggregate.Commands.Evaluators;

public class SetOrderStateResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga,OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult)
        => Task.FromResult(commandResult == expectedResult);
}