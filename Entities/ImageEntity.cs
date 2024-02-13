namespace BiyLineApi.Entities;
public sealed class ImageEntity
{
    public int Id { get; set; }
    public string? FileName { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageMimeType { get; set; }
    public bool? IsMain { get; set; }
    public string? Description { get; set; }
    public DateTime? DateUploaded { get; set; }
    public string? Type { get; set; }

    public int? CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }

    public int? ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int? StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    public int? OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public int? ExpenseId { get; set; }
    public ExpenseEntity? Expense { get; set; }



    public int? ShippingCompanyEntityID { get; set; }
    public  ShippingCompanyEntity? ShippingCompanyEntity { get; set; }

    public int? SubSpecializationId { get; set; }
    public SubSpecializationEntity? SubSpecializationImage { get; set; }
}