using EventDriven.Sagas.Abstractions;

namespace SagaConfigService.Entities;

public class SerializableSagaAction : SagaAction
{
    // public new SagaCommand<object, object> Command { get; set; }
    public new object Command { get; set; }
}