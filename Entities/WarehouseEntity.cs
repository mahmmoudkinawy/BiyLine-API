namespace BiyLineApi.Entities;
public sealed class WarehouseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShippingAddress { get; set; }
    public string? WarehouseStatus { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    public ICollection<StockEntity> SourceStocks { get; set; } = new List<StockEntity>();
    public ICollection<StockEntity> DestinationStocks { get; set; } = new List<StockEntity>();
    public ICollection<WarehouseLogEntity> Logs { get; set; } = new List<WarehouseLogEntity>();
    public ICollection<WarehouseSummaryEntity> Summaries { get; set; } = new List<WarehouseSummaryEntity>();
}
