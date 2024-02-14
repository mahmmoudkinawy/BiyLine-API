namespace BiyLineApi.Entities;
public sealed class CategoryEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<ImageEntity> Images { get; set; }
    public ICollection<ProductEntity> Products { get; set; }
    public ICollection<StoreCategoryEntity> StoreCategories { get; set; }
    public ICollection<SubcategoryEntity> Subcategories { get; set; } = new List<SubcategoryEntity>();
    public  ICollection<CouponCategory> CouponCategories { get; set; }

}