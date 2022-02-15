namespace OrderService.Sagas.Specs.Configuration;

public class OrderServiceSpecsSettings
{
    public Guid SagaConfigId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid InventoryId { get; set; }
    public Guid OrderId { get; set; }
    public string? OrderBaseAddress { get; set; }
    public bool StartTyeProcess { get; set; }
    public TimeSpan TyeProcessTimeout { get; set; }
    public TimeSpan SagaCompletionTimeout { get; set; }
}