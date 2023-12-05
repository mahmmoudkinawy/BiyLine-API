namespace BiyLineApi.Entities;
public sealed class StockEntity
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime Created { get; set; }

    public int SourceWarehouseId { get; set; }
    public WarehouseEntity SourceWarehouse { get; set; }

    public int DestinationWarehouseId { get; set; }
    public WarehouseEntity DestinationWarehouse { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }
}
