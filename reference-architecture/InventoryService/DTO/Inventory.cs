namespace InventoryService.DTO;

public class Inventory
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public int AmountAvailable { get; set; }
    public string ETag { get; set; } = null!;
}