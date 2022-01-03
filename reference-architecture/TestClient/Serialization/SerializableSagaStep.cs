using EventDriven.Sagas.Abstractions;

namespace TestClient.Serialization;

public class SerializableSagaStep : SagaStep
{
    public new SerializableSagaAction Action { get; set; } = null!;
    public new SerializableSagaAction CompensatingAction { get; set; } = null!;
}