namespace BiyLineApi.Entities;
public sealed class StockTrackerEntity
{
    public int Id { get; set; }
    public string? StockTrackerNumber { get; set; }
    public DateTime? Date { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int WarehouseId { get; set; }
    public WarehouseEntity Warehouse { get; set; }
}
