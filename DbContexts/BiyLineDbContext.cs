namespace BiyLineApi.DbContexts;
public sealed class BiyLineDbContext : IdentityDbContext<
    UserEntity, RoleEntity, int, IdentityUserClaim<int>,
    UserRoleEntity, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>
{
    private readonly string _language;
    public BiyLineDbContext(DbContextOptions<BiyLineDbContext> options,
        IAcceptLanguageService acceptLanguageService) : base(options)
    {
        _language = acceptLanguageService.GetLanguageFromHeaderRequest();
    }

    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<OfferEntity> Offers { get; set; }
    public DbSet<StoreEntity> Stores { get; set; }
    public DbSet<RateEntity> Rates { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public DbSet<BasketEntity> Baskets { get; set; }
    public DbSet<BasketItemEntity> BasketItems { get; set; }
    public DbSet<ProductTranslationEntity> ProductTranslations { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<LegalDocumentEntity> LegalDocuments { get; set; }
    public DbSet<GovernorateEntity> Governments { get; set; }
    public DbSet<CountryEntity> Countries { get; set; }
    public DbSet<RegionEntity> Regions { get; set; }
    public DbSet<SpecializationEntity> Specializations { get; set; }
    public DbSet<StoreCategoryEntity> StoreCategories { get; set; }
    public DbSet<StoreProfileCompletenessEntity> StoresProfilesCompleteness { get; set; }
    public DbSet<SubcategoryEntity> Subcategories { get; set; }
    public DbSet<ProductSizeEntity> ProductSizes { get; set; }
    public DbSet<ProductColorEntity> ProductColors { get; set; }
    public DbSet<SubSpecializationEntity> SubSpecializations { get; set; }
    public DbSet<ShippingCompanyEntity> ShippingCompanies { get; set; }
    public DbSet<ShippingCompanyGovernorateEntity> ShippingCompanyGovernorates { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<WarehouseEntity> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WarehouseEntity>()
            .HasMany(w => w.Products)
            .WithOne(p => p.Warehouse)
            .HasForeignKey(w => w.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<UserEntity>()
            .HasOne(u => u.Store)
            .WithOne()
            .HasForeignKey<UserEntity>(k => k.StoreId);

        builder.Entity<CategoryEntity>()
            .HasIndex(c => c.Name);

        builder.Entity<ShippingCompanyGovernorateEntity>()
            .HasKey(scg => new { scg.ShippingCompanyId, scg.GovernorateId });

        builder.Entity<ShippingCompanyGovernorateEntity>()
            .HasOne(scg => scg.ShippingCompany)
            .WithMany(sc => sc.ShippingCompanyGovernorates)
            .HasForeignKey(scg => scg.ShippingCompanyId);

        builder.Entity<ShippingCompanyGovernorateEntity>()
            .HasOne(scg => scg.Governorate)
            .WithMany(g => g.ShippingCompanyGovernorates)
            .HasForeignKey(scg => scg.GovernorateId);

        builder.Entity<ShippingCompanyEntity>()
            .HasOne(sc => sc.Store)
            .WithOne()
            .HasForeignKey<ShippingCompanyEntity>(k => k.StoreId);

        builder.Entity<CouponEntity>()
            .HasIndex(c => c.Code)
            .IsUnique();

        builder.Entity<CouponEntity>()
            .HasOne(c => c.Store)
            .WithMany(s => s.Coupons)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductEntity>()
            .HasMany(p => p.Colors)
            .WithOne()
            .HasForeignKey(p => p.ProductId);

        builder.Entity<ProductEntity>()
            .HasMany(p => p.Sizes)
            .WithOne()
            .HasForeignKey(p => p.ProductId);

        builder.Entity<SubcategoryEntity>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId);

        builder.Entity<ProductEntity>()
            .HasOne(p => p.Subcategory)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SubcategoryId);

        builder.Entity<StoreProfileCompletenessEntity>()
            .HasOne(upc => upc.Store)
            .WithOne(s => s.StoreProfileCompleteness)
            .HasForeignKey<StoreProfileCompletenessEntity>(upc => upc.StoreId);

        builder.Entity<StoreProfileCompletenessEntity>()
            .HasOne(upc => upc.User)
            .WithOne()
            .HasForeignKey<StoreProfileCompletenessEntity>(upc => upc.UserId);

        builder.Entity<SpecializationEntity>()
            .HasMany(s => s.SubSpecializations)
            .WithOne(ss => ss.Specialization)
            .HasForeignKey(s => s.SpecializationId);

        builder.Entity<SubSpecializationEntity>()
            .HasOne(ss => ss.SubSpecializationImage)
            .WithOne(i => i.SubSpecializationImage)
            .HasForeignKey<SubSpecializationEntity>(ss => ss.SubSpecializationImageId);

        builder.Entity<StoreEntity>()
            .HasOne(s => s.Owner)
            .WithOne(o => o.Store)
            .HasForeignKey<StoreEntity>(so => so.OwnerId);

        builder.Entity<StoreCategoryEntity>()
            .HasKey(sc => new { sc.StoreId, sc.CategoryId });

        builder.Entity<StoreCategoryEntity>()
            .HasOne(sc => sc.Store)
            .WithMany(s => s.StoreCategories)
            .HasForeignKey(sc => sc.StoreId);

        builder.Entity<StoreCategoryEntity>()
            .HasOne(sc => sc.Category)
            .WithMany(s => s.StoreCategories)
            .HasForeignKey(sc => sc.CategoryId);

        builder.Entity<StoreEntity>()
            .HasOne(s => s.Country)
            .WithOne(c => c.Store)
            .HasForeignKey<StoreEntity>(s => s.CountryId);

        builder.Entity<SpecializationEntity>()
            .HasOne(s => s.Store)
            .WithMany(s => s.Specializations)
            .HasForeignKey(k => k.StoreId);

        builder.Entity<GovernorateEntity>()
            .HasMany(g => g.Regions)
            .WithOne(r => r.Governorate)
            .HasForeignKey(g => g.GovernorateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GovernorateEntity>()
            .HasOne(g => g.Country)
            .WithMany(c => c.Governorates)
            .HasForeignKey(g => g.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StoreEntity>()
            .HasIndex(s => s.Username)
            .IsUnique();

        builder.Entity<StoreEntity>()
            .HasMany(s => s.StoreCategories)
            .WithOne(sc => sc.Store)
            .HasForeignKey(s => s.StoreId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.TaxRegistrationDocumentImage)
            .WithMany()
            .HasForeignKey(store => store.TaxRegistrationDocumentImageId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.NationalIdImage)
            .WithMany()
            .HasForeignKey(store => store.NationalIdImageId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.Governorate)
            .WithMany()
            .HasForeignKey(store => store.GovernorateId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.Region)
            .WithMany()
            .HasForeignKey(store => store.RegionId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.ProfileCoverImage)
            .WithMany()
            .HasForeignKey(store => store.ProfileCoverImageId);

        builder.Entity<StoreEntity>()
            .HasOne(store => store.ProfilePictureImage)
            .WithMany()
            .HasForeignKey(store => store.ProfilePictureImageId);

        builder.Entity<StoreEntity>()
            .HasMany(s => s.Images)
            .WithOne(i => i.Store)
            .HasForeignKey(s => s.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductTranslationEntity>()
            .Property(pt => pt.Language)
            .HasDefaultValue("ar");

        builder.Entity<ProductEntity>()
            .HasMany(p => p.ProductTranslations)
            .WithOne(t => t.Product)
            .HasForeignKey(pt => pt.ProductId);

        builder.Entity<BasketEntity>()
            .HasOne(u => u.User)
            .WithOne(b => b.Basket)
            .HasForeignKey<BasketEntity>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BasketEntity>()
            .HasMany(b => b.BasketItems)
            .WithOne(bi => bi.Basket)
            .HasForeignKey(b => b.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BasketItemEntity>()
            .HasOne(bi => bi.Product)
            .WithOne()
            .HasForeignKey<BasketItemEntity>(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ImageEntity>()
            .Property(i => i.IsMain)
            .HasDefaultValue(true);

        builder.Entity<RateEntity>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Rates)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductEntity>()
            .HasIndex(p => p.DateAdded);

        builder.Entity<StoreEntity>()
            .HasMany(s => s.Products)
            .WithOne(p => p.Store)
            .HasForeignKey(s => s.StoreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductEntity>()
            .HasOne(p => p.Offer)
            .WithMany(o => o.Products)
            .HasForeignKey(p => p.OfferId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ProductEntity>()
            .HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProductEntity>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ImageEntity>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Images)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<UserEntity>()
            .HasMany(u => u.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.Entity<RoleEntity>()
            .HasMany(u => u.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        builder.Entity<ProductTranslationEntity>()
            .HasQueryFilter(pt => pt.Language == _language);

        builder.Entity<ProductEntity>()
            .HasQueryFilter(c => c.ProductTranslations.Any(pt => pt.Language.Equals(_language)));
    }

}
