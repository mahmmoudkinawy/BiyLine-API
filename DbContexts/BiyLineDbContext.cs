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
    public DbSet<SubSpecializationEntity> SubSpecializations { get; set; }
    public DbSet<ShippingCompanyEntity> ShippingCompanies { get; set; }
    public DbSet<ShippingCompanyGovernorateEntity> ShippingCompanyGovernorates { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<WarehouseEntity> Warehouses { get; set; }
    public DbSet<SupplierEntity> Suppliers { get; set; }
    public DbSet<QuantityPricingTierEntity> QuantityPricingTiers { get; set; }
    public DbSet<ProductVariationEntity> ProductVariations { get; set; }
    public DbSet<ContractOrderProductEntity> ContractOrderProducts { get; set; }
    public DbSet<ContractOrderEntity> ContractOrders { get; set; }
    public DbSet<ContractOrderVariationEntity> ContractOrderVariations { get; set; }
    public DbSet<InventoryEntity> Inventories { get; set; }
    public DbSet<SupplierInvoiceEntity> SupplierInvoices { get; set; }
    public DbSet<StockEntity> Stocks { get; set; }
    public DbSet<StoreWalletEntity> StoreWallets { get; set; }
    public DbSet<CashDepositePermissionEntity> CashDepositePermissions { get; set; }
    public DbSet<CashDiscountPermissionEntity> CashDiscountPermissions { get; set; }
    public DbSet<SalaryPaymentEntity> SalaryPayments { get; set; }
    public DbSet<StockTrackerEntity> StockTrackers { get; set; }
    public DbSet<ExpenseTypeEntity> ExpenseTypes { get; set; }
    public DbSet<ExpenseEntity> Expenses { get; set; }
    public DbSet<TraderShippingCompanyEntity> TraderShippingCompanies { get; set; }
    public DbSet<GovernorateShippingEntity> GovernorateShippings { get; set; }
    public DbSet<StoreChatMessageEntity> StoreMessages { get; set; }
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StoreChatMessageEntity>()
            .HasOne(e => e.SenderUser)
            .WithMany()
            .HasForeignKey(e => e.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StoreChatMessageEntity>()
            .HasOne(e => e.ReceiverUser)
            .WithMany()
            .HasForeignKey(e => e.ReceiverUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StoreChatMessageEntity>()
            .HasOne(e => e.Store)
            .WithMany()
            .HasForeignKey(e => e.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ExpenseEntity>()
            .HasOne(e => e.ExpenseType)
            .WithOne()
            .HasForeignKey<ExpenseEntity>(e => e.ExpenseTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ExpenseEntity>()
            .HasOne(e => e.Store)
            .WithMany()
            .HasForeignKey(e => e.StoreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ExpenseEntity>()
            .HasOne(e => e.StoreWallet)
            .WithMany()
            .HasForeignKey(e => e.StoreWalletId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ExpenseEntity>()
            .HasOne(e => e.ReceiptImage)
            .WithOne()
            .HasForeignKey<ExpenseEntity>(e => e.ReceiptImageId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ExpenseTypeEntity>()
            .HasOne(e => e.Store)
            .WithMany()
            .HasForeignKey(e => e.StoreId);

        builder.Entity<ExpenseTypeEntity>()
            .HasOne(e => e.StoreWallet)
            .WithMany()
            .HasForeignKey(e => e.StoreWalletId);

        builder.Entity<ProductEntity>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<StockTrackerEntity>()
            .HasOne(st => st.Store)
            .WithMany()
            .HasForeignKey(st => st.StoreId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<StockTrackerEntity>()
            .HasOne(st => st.Warehouse)
            .WithMany()
            .HasForeignKey(st => st.WarehouseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<StockEntity>()
            .HasOne(s => s.Store)
            .WithMany(s => s.Stocks)
            .HasForeignKey(k => k.StoreId);

        builder.Entity<StockEntity>()
            .HasOne(stock => stock.Product)
            .WithMany(product => product.Stocks)
            .HasForeignKey(stock => stock.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockEntity>()
            .HasOne(stock => stock.SourceWarehouse)
            .WithMany(warehouse => warehouse.SourceStocks)
            .HasForeignKey(stock => stock.SourceWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockEntity>()
            .HasOne(stock => stock.DestinationWarehouse)
            .WithMany(warehouse => warehouse.DestinationStocks)
            .HasForeignKey(stock => stock.DestinationWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryEntity>()
            .HasOne(i => i.Warehouse)
            .WithOne()
            .HasForeignKey<InventoryEntity>(i => i.WarehouseId);


        builder.Entity<QuantityPricingTierEntity>()
            .HasOne(qpt => qpt.Product)
            .WithMany(p => p.QuantityPricingTiers)
            .HasForeignKey(k => k.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .HasMany(p => p.ProductVariations)
            .WithOne(pv => pv.Product)
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
            .WithMany(c => c.Stores)
            .HasForeignKey(k => k.CountryId);

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

        builder.Entity<ContractOrderEntity>()
             .HasOne(e => e.FromStore)
             .WithMany(s => s.ContractOrdersFromStore)
             .HasForeignKey(e => e.FromStoreId)
             .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ContractOrderEntity>()
            .HasOne(c => c.ToStore)
            .WithMany(s => s.ContractOrdersToStore)
            .HasForeignKey(c => c.ToStoreId)
            .IsRequired();

        builder.Entity<ContractOrderEntity>()
            .HasMany(c => c.ContractOrderProducts)
            .WithMany(c => c.ContractOrders);

        builder.Entity<ContractOrderProductEntity>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.ContractOrderProducts)
            .HasForeignKey(cp => cp.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.NoAction);

        builder.Entity<ContractOrderVariationEntity>()
            .HasOne(c => c.ProductVariation)
            .WithMany(p => p.ContractOrderVariations)
            .HasForeignKey(s => s.ProductVariationId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<SupplierEntity>()
              .HasOne(s => s.Store)
              .WithMany(s => s.Suppliers)
              .HasForeignKey(s => s.StoreId);

        builder.Entity<StoreWalletEntity>()
            .HasOne(s => s.Store)
            .WithMany(s => s.StoreWallets)
            .HasForeignKey(s => s.StoreId)
            .OnDelete(DeleteBehavior.NoAction);


    }
}
