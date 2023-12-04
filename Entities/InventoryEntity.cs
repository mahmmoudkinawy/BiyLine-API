namespace BiyLineApi.Entities;
public sealed class InventoryEntity
{
    public int Id { get; set; }
    public string? CodeNumber { get; set; }
    public DateTime? Created { get; set; }

    public int WarehouseId { get; set; }
    public WarehouseEntity Warehouse { get; set; }
}