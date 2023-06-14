using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;

namespace EventDriven.Sagas.Persistence.Abstractions.Tests.Fakes;

public class FakeSaga : PersistableSaga<FakeMetadata>, ISagaCommandResultHandler
{
    public FakeSaga(
        FakeMetadata metadata,
        List<SagaStep> steps,
        ISagaCommandDispatcher sagaCommandDispatcher, 
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators, 
        ISagaPool sagaPool) : base(sagaCommandDispatcher, commandResultEvaluators, sagaPool)
    {
        Metadata = metadata;
        Steps = steps;
    }

    protected override Task<bool> CheckLock(Guid entityId) => throw new NotImplementedException();
}