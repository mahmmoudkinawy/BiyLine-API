namespace BiyLineApi.Entities;
public sealed class ProductTranslationEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? GeneralOverview { get; set; }
    public string? Specifications { get; set; }

    public string Language { get; set; }

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; }
}
