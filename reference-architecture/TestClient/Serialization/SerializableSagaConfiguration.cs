using EventDriven.Sagas.Abstractions.Repositories;

namespace TestClient.Serialization;

public class SerializableSagaConfiguration : SagaConfiguration
{
    public new Dictionary<int, SerializableSagaStep> Steps { get; set; } = null!;
}