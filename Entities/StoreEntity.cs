namespace BiyLineApi.Entities;
public sealed class StoreEntity
{
    public int Id { get; set; }
    public string? ArabicName { get; set; }
    public string? EnglishName { get; set; }
    public string? Username { get; set; }
    public string? Address { get; set; }
    public bool? AcceptsReturns { get; set; }
    public decimal? Rates { get; set; } // Will be replaced 
    public int? ExperienceInYears { get; set; }
    public int? NumberOfEmployees { get; set; }
    public double? Rating { get; set; }
    public int? MinimumNumberOfPieces { get; set; }
    public string? Activity { get; set; }

    public ICollection<ProductEntity> Products { get; set; }
    public ICollection<ImageEntity> Images { get; set; }
    public ICollection<SpecializationEntity> Specializations { get; set; } = new List<SpecializationEntity>();
    public ICollection<StoreCategoryEntity> StoreCategories { get; set; } = new List<StoreCategoryEntity>();
    public ICollection<CouponEntity> Coupons { get; set; } = new List<CouponEntity>();
    public ICollection<EmployeeEntity> Employees { get; set; }

    public int? TaxRegistrationDocumentImageId { get; set; }
    public ImageEntity? TaxRegistrationDocumentImage { get; set; }

    public int? ProfileCoverImageId { get; set; }
    public ImageEntity? ProfileCoverImage { get; set; }

    public int? ProfilePictureImageId { get; set; }
    public ImageEntity? ProfilePictureImage { get; set; }

    public int? NationalIdImageId { get; set; }
    public ImageEntity? NationalIdImage { get; set; }

    public int? CountryId { get; set; }
    public CountryEntity? Country { get; set; }

    public int? GovernorateId { get; set; }
    public GovernorateEntity? Governorate { get; set; }

    public int? RegionId { get; set; }
    public RegionEntity? Region { get; set; }

    public int? OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public StoreProfileCompletenessEntity StoreProfileCompleteness { get; set; }

    public ICollection<SupplierEntity> Suppliers { get; set; }
}
