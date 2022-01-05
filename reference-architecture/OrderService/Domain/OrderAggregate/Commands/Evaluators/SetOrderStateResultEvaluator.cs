using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands.Evaluators;

public class SetOrderStateResultEvaluator : CommandResultEvaluator<OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult)
        => Task.FromResult(commandResult == expectedResult);
}