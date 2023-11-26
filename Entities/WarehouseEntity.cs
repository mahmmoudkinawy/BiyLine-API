namespace BiyLineApi.Entities;
public sealed class WarehouseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShippingAddress { get; set; }
    public string? WarehouseStatus { get; set; }

    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}
