using Entities = EventDriven.Sagas.Abstractions;

namespace TestClient.DTO;

public class SagaConfiguration : Entities.SagaConfiguration
{
    public new Dictionary<int, SagaStep> Steps { get; set; } = null!;
}