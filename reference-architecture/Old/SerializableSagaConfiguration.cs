namespace SagaConfigService.Entities;

public class SerializableSagaConfiguration : SagaConfiguration
{
    public new Dictionary<int, SerializableSagaStep> Steps { get; set; } = null!;
}