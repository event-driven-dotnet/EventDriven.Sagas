namespace TestClient.Configuration;

public class SagaConfigServiceSettings
{
    public string ServiceUri { get; set; } = null!;
    public Guid SagaConfigId { get; set; }
    public string SagaConfigPath { get; set; } = null!;
    public bool Debug { get; set; }
}