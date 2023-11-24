namespace BiyLineApi.Entities;
public sealed class StoreCategoryEntity
{
    public int StoreId { get; set; }
    public StoreEntity Store { get; set; }

    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; }
}
