using EventDriven.Sagas.Abstractions;

namespace TestClient.Serialization;

public class SerializableSagaAction : SagaAction
{
    public new object Command { get; set; }
}