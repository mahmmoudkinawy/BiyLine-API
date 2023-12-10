using Bogus;

namespace BiyLineApi.DbContexts;
public static class Seed
{
    public static async Task SeedRolesAsync(RoleManager<RoleEntity> roleManager)
    {
        if (await roleManager.RoleExistsAsync(Constants.Roles.Admin))
        {
            return;
        }

        var roles = new List<RoleEntity>()
        {
            new RoleEntity
            {
                Name = Constants.Roles.Admin
            },
            new RoleEntity
            {
                Name = Constants.Roles.Customer
            },
            new RoleEntity
            {
                Name = Constants.Roles.Trader
            },
            new RoleEntity
            {
                Name = Constants.Roles.Representative
            },
            new RoleEntity
            {
                Name = Constants.Roles.ShippingCompany
            },
            new RoleEntity
            {
                Name = Constants.Roles.Manager
            },
            new RoleEntity
            {
                Name = Constants.Roles.Employee
            }
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    public static async Task SeedUsersAsync(UserManager<UserEntity> userManager)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var customer = new UserEntity
        {
            Name = "customer",
            Email = "customer@test.com",
            UserName = "customer@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(customer, "Pa$$w0rd");
        await userManager.AddToRoleAsync(customer, Constants.Roles.Customer);

        var admin = new UserEntity
        {
            Name = "admin",
            Email = "admin@test.com",
            UserName = "admin@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRoleAsync(admin, Constants.Roles.Admin);

        var trader = new UserEntity
        {
            Name = "trader",
            Email = "trader@test.com",
            UserName = "trader@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(trader, "Pa$$w0rd");
        await userManager.AddToRoleAsync(trader, Constants.Roles.Trader);

        var representative = new UserEntity
        {
            Name = "representative",
            Email = "representative@test.com",
            UserName = "representative@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(representative, "Pa$$w0rd");
        await userManager.AddToRoleAsync(representative, Constants.Roles.Representative);

        var shippingCompany = new UserEntity
        {
            Name = "shippingCompany",
            Email = "shippingCompany@test.com",
            UserName = "shippingCompany@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(shippingCompany, "Pa$$w0rd");
        await userManager.AddToRoleAsync(shippingCompany, Constants.Roles.ShippingCompany);

        var supplier = new UserEntity
        {
            Name = "supplier",
            Email = "supplier@test.com",
            UserName = "supplier@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(supplier, "Pa$$w0rd");
        await userManager.AddToRoleAsync(supplier, Constants.Roles.Trader);

        var employee = new UserEntity
        {
            Name = "employee",
            Email = "employee@test.com",
            UserName = "employee@test.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(employee, "Pa$$w0rd");
        await userManager.AddToRoleAsync(employee, Constants.Roles.Employee);



    }

    public static async Task SeedCategoriesWithRelatedDataAsync(BiyLineDbContext context)
    {
        if (await context.Categories.AnyAsync())
        {
            return;
        }

        var categories = new Faker<CategoryEntity>()
             .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
             .RuleFor(c => c.Description, f => f.Lorem.Sentence())
             .Generate(50);

        for (int i = 0; i < categories.Count; i++)
        {
            var category = categories[i];

            var categoryImages = new List<ImageEntity>();

            for (int j = 0; j < 3; j++)
            {
                var image = new Faker<ImageEntity>()
                    .RuleFor(img => img.FileName, f => f.Lorem.Word())
                    .RuleFor(img => img.ImageUrl, f => f.Image.PicsumUrl())
                    .RuleFor(img => img.ImageMimeType, "image/jpeg")
                    .RuleFor(img => img.IsMain, f => j == 0)
                    .RuleFor(img => img.Description, f => f.Lorem.Text())
                    .RuleFor(img => img.DateUploaded, f => f.Date.Past())
                    .Generate();

                categoryImages.Add(image);
            }

            category.Images = categoryImages;

            var subcategories = new Faker<SubcategoryEntity>()
                .RuleFor(sub => sub.Name, f => f.Commerce.Categories(1)[0])
                .Generate(3);

            foreach (var subcategory in subcategories)
            {
                category.Subcategories.Add(subcategory);
            }
        }

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var offers = new Faker<OfferEntity>()
            .RuleFor(o => o.DiscountPercentage, f => f.Random.Decimal(5, 50))
            .RuleFor(o => o.StartDate, f => f.Date.Recent())
            .RuleFor(o => o.EndDate, f => f.Date.Future())
            .Generate(5);

        context.Offers.AddRange(offers);
        await context.SaveChangesAsync();

        var imageStoreFaker = new Faker<ImageEntity>()
            .RuleFor(img => img.FileName, f => f.Lorem.Word())
            .RuleFor(i => i.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(i => i.ImageMimeType, "image/png")
            .RuleFor(i => i.IsMain, (f, i) => f.PickRandom(0, 1, 2) == 0)
            .RuleFor(i => i.Description, f => f.Lorem.Text())
            .RuleFor(i => i.DateUploaded, f => f.Date.Past());

        var stores = new Faker<StoreEntity>()
            .RuleFor(s => s.ArabicName, f => f.Company.CompanyName())
            .RuleFor(s => s.EnglishName, f => f.Company.CompanyName())
            .RuleFor(s => s.Rates, f => f.Random.Decimal(1, 5))
            .RuleFor(s => s.ExperienceInYears, f => f.Random.Int(1, 30))
            .RuleFor(s => s.NumberOfEmployees, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Rating, f => f.Random.Double(1, 5))
            .RuleFor(s => s.MinimumNumberOfPieces, f => f.Random.Int(1, 8))
            .RuleFor(s => s.Images, () => new List<ImageEntity> { imageStoreFaker.Generate() })
            .Generate(10);

        context.Stores.AddRange(stores);
        await context.SaveChangesAsync();

        var products = new List<ProductEntity>();

        var englishFaker = new Faker<ProductEntity>("en");
        var arabicFaker = new Faker<ProductEntity>("ar");

        var arabicTranslationFaker = new Faker<ProductTranslationEntity>("ar");
        var englishTranslationFaker = new Faker<ProductTranslationEntity>("en");

        for (var i = 0; i < 60; i++)
        {
            var arabicTranslation = arabicTranslationFaker
                .RuleFor(t => t.Language, f => "ar")
                .RuleFor(t => t.Name, f => f.Lorem.Word())
                .RuleFor(t => t.Description, f => f.Lorem.Sentence())
                .RuleFor(t => t.Brand, f => f.Company.CompanyName())
                .RuleFor(p => p.GeneralOverview, f => $"<head><title>{f.Lorem.Word()}</title></head><body><section>{f.Lorem.Paragraphs(f.Random.Int(1, 4), "<span></span>")}</section></body>")
                .RuleFor(t => t.Specifications, f => $"<head><title>{f.Lorem.Word()}</title></head><body><div>{f.Lorem.Paragraphs(4, "<p></p>")}</div></body>")
                .Generate(1);

            var englishTranslation = englishTranslationFaker
                .RuleFor(t => t.Language, f => "en")
                .RuleFor(t => t.Name, f => f.Lorem.Word())
                .RuleFor(t => t.Description, f => f.Lorem.Sentence())
                .RuleFor(t => t.Brand, f => f.Company.CompanyName())
                .RuleFor(p => p.GeneralOverview, f => $"<head><title>{f.Lorem.Word()}</title></head><body><section>{f.Lorem.Paragraphs(f.Random.Int(1, 4), "<span></span>")}</section></body>")
                .RuleFor(t => t.Specifications, f => $"<head><title>{f.Lorem.Word()}</title></head><body><div>{f.Lorem.Paragraphs(4, "<p></p>")}</div></body>")
                .Generate(1);

            var englishProduct = englishFaker
                .RuleFor(p => p.OriginalPrice, f => f.Random.Decimal(10, 100))
                .RuleFor(p => p.SellingPrice, f => f.Random.Decimal(100, 10000))
                .RuleFor(p => p.ThresholdReached, f => f.Random.Int(5, 50))
                .RuleFor(p => p.CodeNumber, f => f.Random.AlphaNumeric(10))
                .RuleFor(p => p.IsInStock, f => i % 2 == 0)
                .RuleFor(p => p.Vat, f => f.Random.Decimal(1, 14))
                .RuleFor(p => p.Weight, f => f.Random.Decimal(0.1M, 10))
                .RuleFor(p => p.Dimensions, f =>
                    $"{f.Random.Int(1, 10)}M x {f.Random.Int(1, 10)}M x {f.Random.Float(1, 10)}CM")
                .RuleFor(p => p.CountInStock, f => f.Random.Int(0, 120))
                .RuleFor(p => p.NumberOfReviews, f => f.Random.Int(0, 100))
                .RuleFor(p => p.WarrantyMonths, f => f.Random.Int(1, 13))
                .RuleFor(p => p.DateAdded, f => f.Date.Past())
                .RuleFor(p => p.CategoryId, (f, p) => f.PickRandom(categories).Id)
                .RuleFor(p => p.OfferId, (f, p) => f.Random.Int(0, 2) == 0 ? null : f.PickRandom(offers).Id)
                .RuleFor(p => p.StoreId, (f, p) => f.Random.Int(0, 2) == 0 ? null : f.PickRandom(stores).Id)
                .Generate();

            var arabicProduct = arabicFaker
                .RuleFor(p => p.OriginalPrice, f => f.Random.Decimal(10, 100))
                .RuleFor(p => p.SellingPrice, f => f.Random.Decimal(100, 10000))
                .RuleFor(p => p.ThresholdReached, f => f.Random.Int(5, 50))
                .RuleFor(p => p.CodeNumber, f => f.Random.AlphaNumeric(10))
                .RuleFor(p => p.IsInStock, f => i % 2 != 0)
                .RuleFor(p => p.Vat, f => f.Random.Decimal(1, 14))
                .RuleFor(p => p.Weight, f => f.Random.Decimal(0.1M, 10))
                .RuleFor(p => p.Dimensions, f =>
                    $"{f.Random.Int(1, 10)}M x {f.Random.Int(1, 10)}M x {f.Random.Float(1, 10)}CM")
                .RuleFor(p => p.CountInStock, f => f.Random.Int(0, 120))
                .RuleFor(p => p.NumberOfReviews, f => f.Random.Int(0, 100))
                .RuleFor(p => p.WarrantyMonths, f => f.Random.Int(1, 13))
                .RuleFor(p => p.DateAdded, f => f.Date.Past())
                .RuleFor(p => p.CategoryId, (f, p) => f.PickRandom(categories).Id)
                .RuleFor(p => p.OfferId, (f, p) => f.Random.Int(0, 2) == 0 ? null : f.PickRandom(offers).Id)
                .RuleFor(p => p.StoreId, (f, p) => f.Random.Int(0, 2) == 0 ? null : f.PickRandom(stores).Id)
                .Generate();

            englishProduct.ProductTranslations = englishTranslation.Concat(arabicTranslation).ToList();

            arabicProduct.ProductTranslations = arabicTranslation.Concat(englishTranslation).ToList();

            products.Add(englishProduct);
            products.Add(arabicProduct);
        }

        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];

            var productImages = new List<ImageEntity>();

            for (int j = 0; j < 3; j++)
            {
                var image = new Faker<ImageEntity>(j == 0 ? "ar" : "en")
                    .RuleFor(i => i.FileName, f => f.Lorem.Word())
                    .RuleFor(i => i.ImageUrl, f => f.Image.PicsumUrl())
                    .RuleFor(i => i.ImageMimeType, "image/jpeg")
                    .RuleFor(img => img.IsMain, f => j == 0)
                    .RuleFor(i => i.Description, f => f.Lorem.Text())
                    .RuleFor(i => i.DateUploaded, f => f.Date.Past())
                    .Generate();

                productImages.Add(image);
            }

            var rates = new List<RateEntity>();

            for (int j = 0; j < 12; j++)
            {
                var rate = new Faker<RateEntity>(j % 2 == 0 ? "ar" : "en")
                    .RuleFor(r => r.Rating, f => f.Random.Decimal(1, 5))
                    .RuleFor(r => r.RatingDate, f => f.Date.Past())
                    .RuleFor(r => r.Review, f => f.Lorem.Paragraph())
                    .Generate();

                rates.Add(rate);
            }

            product.Images = productImages;
            product.Rates = rates;
        }

        var coupons = new Faker<CouponEntity>()
                 .RuleFor(c => c.Code, f => f.Lorem.Word())
                 .RuleFor(c => c.EndDate, f => f.Date.Future())
                 .RuleFor(c => c.StartDate, f => f.Date.Past())
                 .RuleFor(c => c.DiscountAmount, f => f.Finance.Amount(0, 123))
                 .Generate(10);

        var documents = new List<LegalDocumentEntity>();

        for (int i = 0; i < 2; i++)
        {
            var documentFaker = new Faker<LegalDocumentEntity>("ar")
                .RuleFor(ld => ld.Title, f => f.Lorem.Word())
                .RuleFor(p => p.Content, f => $"<head><title>{f.Lorem.Word()}</title></head><body><section>{f.Lorem.Paragraphs(f.Random.Int(1, 4), "<span></span>")}</section></body>")
                .RuleFor(ld => ld.Type, f => i == 0 ? "TermsAndConditions" : "Policies")
                .RuleFor(ld => ld.UploadDate, f => f.Date.Past())
                .RuleFor(ld => ld.UploadDate, f => f.Date.Future())
                .Generate();

            documents.Add(documentFaker);
        }

        context.LegalDocuments.AddRange(documents);
        context.Coupons.AddRange(coupons);
        context.Products.AddRange(products);

        await context.SaveChangesAsync();
    }

    public static async Task SeedCountriesWithGovernoratesAndRegionsAsync(
        BiyLineDbContext context)
    {
        if (await context.Countries.AnyAsync())
        {
            return;
        }

        var egypt = new CountryEntity
        {
            Name = "Egypt",
            CountryCode = "EG",
            CurrencyCode = "EGP",
            CurrencySymbol = "£",
            Governorates = new List<GovernorateEntity>
            {
                new GovernorateEntity
                {
                    Name = "Cairo",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Cairo City" },
                        new RegionEntity { Name = "Giza" },
                        new RegionEntity { Name = "Heliopolis" },
                        new RegionEntity { Name = "Nasr City" },
                        new RegionEntity { Name = "Maadi" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Alexandria",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Alexandria City" },
                        new RegionEntity { Name = "Borg El Arab" },
                        new RegionEntity { Name = "Amreya" },
                        new RegionEntity { Name = "Montaza" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Giza",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Giza City" },
                        new RegionEntity { Name = "6th of October City" },
                        new RegionEntity { Name = "Sheikh Zayed City" },
                        new RegionEntity { Name = "Dokki" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Menofia",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Shibin El Kom" },
                        new RegionEntity { Name = "Menouf" },
                        new RegionEntity { Name = "Ashmoun" }
                    }
                },
            }
        };

        var jordan = new CountryEntity
        {
            Name = "Jordan",
            CountryCode = "JO",
            CurrencyCode = "JOD",
            CurrencySymbol = "د.ا",
            Governorates = new List<GovernorateEntity>
            {
                new GovernorateEntity
                {
                    Name = "Amman",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Amman City" },
                        new RegionEntity { Name = "Zarqa" },
                        new RegionEntity { Name = "Madaba" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Irbid",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Irbid City" },
                        new RegionEntity { Name = "Mafraq" },
                        new RegionEntity { Name = "Ramtha" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Aqaba",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Aqaba City" },
                        new RegionEntity { Name = "Eilat" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Karak",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Karak City" },
                        new RegionEntity { Name = "Tafila" }
                    }
                },
                new GovernorateEntity
                {
                    Name = "Ma'an",
                    Regions = new List<RegionEntity>
                    {
                        new RegionEntity { Name = "Ma'an City" },
                        new RegionEntity { Name = "Petra" }
                    }
                },
            }
        };

        context.Countries.AddRange(egypt, jordan);
        await context.SaveChangesAsync();
    }
}
