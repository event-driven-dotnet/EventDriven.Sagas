using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Persistence.Abstractions;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder;

public class CreateOrderSaga : PersistableSaga, ISagaCommandResultHandler
{
    public CreateOrderSaga(ISagaCommandDispatcher sagaCommandDispatcher, IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) : base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }

    protected override Task<bool> CheckLock(Guid entityId)
    {
        throw new NotImplementedException();
    }
}