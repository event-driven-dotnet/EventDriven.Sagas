using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Evaluators;

public class SetOrderStateResultEvaluator : SagaCommandResultEvaluator<CreateOrderSaga, OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult)
    {
        throw new NotImplementedException();
    }
}