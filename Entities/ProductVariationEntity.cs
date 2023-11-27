namespace BiyLineApi.Entities;
public sealed class ProductVariationEntity
{
    public int Id { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public int? Quantity { get; set; }
    public decimal? Price { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }
}