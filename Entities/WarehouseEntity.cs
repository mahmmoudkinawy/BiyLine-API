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
    public ICollection<StockEntity> SourceStocks { get; set; }
    public ICollection<StockEntity> DestinationStocks { get; set; }
    public ICollection<WarehouseLogEntity> Logs { get; set; }
}
