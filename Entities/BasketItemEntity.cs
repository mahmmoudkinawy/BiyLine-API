namespace BiyLineApi.Entities;
public sealed class BasketItemEntity
{
    public int Id { get; set; }
    public int? Quantity { get; set; }
    public string? Name { get; set; }
    public string? ImageSrc { get; set; }
    public decimal Price { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }

    public ProductEntity? Product { get; set; }
    public int? ProductId { get; set; }

    public BasketEntity? Basket { get; set; }
    public int? BasketId { get; set; }
}
