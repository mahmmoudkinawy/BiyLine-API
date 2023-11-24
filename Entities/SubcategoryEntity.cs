namespace BiyLineApi.Entities;
public sealed class SubcategoryEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; }

    public ICollection<ProductEntity> Products { get; set; }
}