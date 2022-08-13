using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Evaluators;

public class SetOrderStateResultEvaluator2 : SagaCommandResultEvaluator<CreateOrderSaga, OrderState, OrderState>
{
    public override Task<bool> EvaluateCommandResultAsync(OrderState commandResult, OrderState expectedResult)
    {
        throw new NotImplementedException();
    }
}