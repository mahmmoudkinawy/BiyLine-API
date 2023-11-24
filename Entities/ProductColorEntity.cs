namespace BiyLineApi.Entities;
public sealed class ProductColorEntity
{
    public int Id { get; set; }
    public string? Color { get; set; }
    public int? Quantity { get; set; }

    public int ProductId { get; set; }
}
