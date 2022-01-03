using EventDriven.Sagas.Abstractions;

namespace SagaConfigService.Entities;

public class SerializableSagaStep : SagaStep
{
    public new SerializableSagaAction Action { get; set; } = null!;
    public new SerializableSagaAction CompensatingAction { get; set; } = null!;
}