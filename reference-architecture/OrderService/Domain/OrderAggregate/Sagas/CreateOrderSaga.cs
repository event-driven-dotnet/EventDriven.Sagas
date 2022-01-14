using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Persistence.Abstractions;

namespace OrderService.Domain.OrderAggregate.Sagas;

public class CreateOrderSaga :
    PersistableSaga,
    ISagaCommandResultHandler<OrderState>
{
    public CreateOrderSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) :
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }
    
    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        var action = step.Action;
        if (action.Command is SagaCommand<OrderState, OrderState> command)
            command.Result = result;
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(step, compensating);
    }
}