namespace BiyLineApi.Entities;
public sealed class ProductEntity
{
        public int Id { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public int? CountInStock { get; set; }
        public bool? IsInStock { get; set; }
        public int? NumberOfReviews { get; set; }
        public int? WarrantyMonths { get; set; }
        public string? CodeNumber { get; set; }
        public decimal? Vat { get; set; }
        public int? ThresholdReached { get; set; }
        public DateTime? DateAdded { get; set; }

        public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; }

    public int? OfferId { get; set; }
    public OfferEntity? Offer { get; set; }

    public int? StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    public int? SubcategoryId { get; set; }
    public SubcategoryEntity? Subcategory { get; set; }

    public int? WarehouseId { get; set; }
    public WarehouseEntity? Warehouse { get; set; }
    
    public ICollection<ImageEntity> Images { get; set; }
    public ICollection<RateEntity> Rates { get; set; }
    public ICollection<ProductVariationEntity> ProductVariations { get; set; } = new List<ProductVariationEntity>();
    public ICollection<QuantityPricingTierEntity> QuantityPricingTiers { get; set; } = new List<QuantityPricingTierEntity>();
    public ICollection<ProductTranslationEntity> ProductTranslations { get; set; } = new List<ProductTranslationEntity>();
    public ICollection<ContractOrderProductEntity> ContractOrderProducts { get; set; } = new List<ContractOrderProductEntity>();

}