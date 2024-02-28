namespace BiyLineApi.Features.Products.Commands;
public sealed class CreateProductFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public int WarehouseId { get; set; }
        public string? ArabicName { get; set; }
        public string? EnglishName { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? CodeNumber { get; set; }
        public decimal? Vat { get; set; }
        public int? ThresholdReached { get; set; }
        public List<IFormFile>? Images { get; set; }
        public string? GeneralOverview { get; set; }
        public string? ArabicDescription { get; set; }
        public string? EnglishDescription { get; set; }
        public bool? IsInStock { get; set; }
        public int? CountInStock { get; set; }
        public List<ProductVariationRequest>? Variations { get; set; }
        public List<QuantityPricingTierRequest>? PricingTiers { get; set; }
    }

    public sealed class ProductVariationRequest
    {
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int? Quantity { get; set; }
    }

    public sealed class QuantityPricingTierRequest
    {
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public decimal? Price { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Validator(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            When(r => r.CategoryId != null, () =>
            {
                RuleFor(r => r.CategoryId)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0)
                    .Must((req, _) => _context.Categories.Any(c => c.Id == req.CategoryId))
                        .WithMessage("Category with the given Id does not exist");
            });

            RuleFor(r => r.WarehouseId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            When(r => r.SubcategoryId != null, () =>
            {
                RuleFor(r => r.SubcategoryId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .Must((req, _) =>
                        _context.Subcategories.Any(c => c.Id == req.SubcategoryId) &&
                        _context.Categories.Any(c => c.Id == req.CategoryId && c.Subcategories.Any(sc => sc.Id == req.SubcategoryId)))
                    .WithMessage("SubcategoryId with the given Id does not exist");
            });

            RuleFor(r => r.ArabicName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(255);

            RuleFor(r => r.EnglishName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(255);

            RuleFor(r => r.OriginalPrice)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.SellingPrice)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.CodeNumber)
                .NotEmpty()
                .NotNull()
                .MaximumLength(500);

            RuleFor(r => r.Vat)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            When(req => req.ThresholdReached != null, () =>
            {
                RuleFor(r => r.ThresholdReached)
                    .GreaterThan(0);

                RuleFor(r => r.ThresholdReached)
                    .LessThanOrEqualTo(r => r.CountInStock);
            });

            When(req => req.Images != null && req.Images.Any(), () =>
            {
                RuleFor(r => r.Images)
                    .Must(images => images.Count <= 5)
                        .WithMessage("Maximum of 5 images allowed");

                RuleForEach(r => r.Images)
                    .Must(IsValidImage)
                        .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
            });

            RuleFor(r => r.GeneralOverview)
                .NotEmpty()
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(5000);

            RuleFor(r => r.ArabicDescription)
                .NotEmpty()
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(5000);

            RuleFor(r => r.EnglishDescription)
                .NotEmpty()
                .NotNull()
                .MinimumLength(10)
                .MaximumLength(5000);

            RuleFor(r => r.IsInStock)
                .NotEmpty()
                .NotNull();

            RuleFor(r => r.CountInStock)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0);

            When(x => x.Variations != null, () =>
            {
                RuleForEach(x => x.Variations)
                    .SetValidator(new ProductVariationRequestValidator());
            });

            When(x => x.PricingTiers != null && x.PricingTiers.Any(), () =>
            {
                RuleForEach(x => x.PricingTiers)
                    .SetValidator(new QuantityPricingTierRequestValidator());
            });

            When(x => x.PricingTiers != null && IsUserWholesaleFactoryImporter(), () =>
            {
                RuleFor(x => x.PricingTiers)
                    .Must(p => p?.Count > 0)
                        .WithMessage("At least one pricing tier is required for Wholesale, Factory, Importer users");
            });
        }

        private bool IsUserWholesaleFactoryImporter()
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = _context.Stores.FirstOrDefault(s => s.OwnerId == userId);

            return store.Activity == StoreActivityEnum.Wholesale.ToString() ||
                   store.Activity == StoreActivityEnum.Factory.ToString() ||
                   store.Activity == StoreActivityEnum.Importer.ToString();
        }

        public sealed class QuantityPricingTierRequestValidator : AbstractValidator<QuantityPricingTierRequest>
        {
            public QuantityPricingTierRequestValidator()
            {
                RuleFor(x => x.MinQuantity)
                    .NotNull()
                        .WithMessage("MinQuantity is required")
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("MinQuantity must be greater than or equal to 0");

                RuleFor(x => x.MaxQuantity)
                    .NotNull()
                        .WithMessage("MaxQuantity is required")
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("MaxQuantity must be greater than or equal to 0")
                    .GreaterThanOrEqualTo(x => x.MinQuantity)
                        .WithMessage("MaxQuantity must be greater than or equal to MinQuantity");

                RuleFor(x => x.Price)
                    .NotNull()
                        .WithMessage("Price is required")
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("Price must be greater than or equal to 0");
            }
        }
        public sealed class ProductVariationRequestValidator : AbstractValidator<ProductVariationRequest>
        {
            public ProductVariationRequestValidator()
            {
                RuleFor(x => x.Color)
                    .NotEmpty()
                        .WithMessage("Color is required");

                RuleFor(x => x.Size)
                    .NotEmpty()
                        .WithMessage("Size is required");

                RuleFor(x => x.Quantity)
                    .NotNull()
                        .WithMessage("Quantity is required")
                    .GreaterThanOrEqualTo(0)
                        .WithMessage("Quantity must be greater than or equal to 0");
            }
        }

        private static bool IsValidImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return false;
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                return false;
            }

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName);
            return validExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IDateTimeProvider dateTimeProvider,
            IImageService imageService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var warehouse = await _context.Warehouses
                .AnyAsync(w => w.Id == request.WarehouseId && w.StoreId == store.Id, cancellationToken: cancellationToken);

            if (!warehouse)
            {
                return Result<Response>.Failure(new List<string> { "Warehouse does not exists for this store." });
            }

            var productToCreate = new ProductEntity
            {
                StoreId = store.Id,
                WarehouseId = request.WarehouseId,
                CategoryId = request.CategoryId ?? null,
                CodeNumber = request.CodeNumber,
                ThresholdReached = request.ThresholdReached,
                CountInStock = request.CountInStock,
                IsInStock = request.IsInStock,
                SellingPrice = request.SellingPrice,
                OriginalPrice = request.OriginalPrice,
                Vat = request.Vat,
                SubcategoryId = request.SubcategoryId ?? null,
                DateAdded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                ProductTranslations = new List<ProductTranslationEntity>
                {
                    new ProductTranslationEntity
                    {
                        Language = "ar",
                        Name = request.ArabicName,
                        Description = request.ArabicDescription,
                        GeneralOverview = request.GeneralOverview
                    },
                    new ProductTranslationEntity
                    {
                        Language = "en",
                        Name = request.EnglishName,
                        Description = request.EnglishDescription,
                        GeneralOverview = request.GeneralOverview
                    }
                },
                ProductVariations = request.Variations != null ? request.Variations.Select(req => new ProductVariationEntity
                {
                    Color = req.Color,
                    Quantity = req.Quantity,
                    Size = req.Size
                }).ToList() : null
            };

            if (request.Images != null)
            {
                var images = await Task.WhenAll(request.Images.Select(async image =>
                {
                    return new ImageEntity
                    {
                        ProductId = productToCreate.Id,
                        StoreId = store.Id,
                        ImageMimeType = image.ContentType,
                        FileName = image.FileName,
                        Type = "ProductImage",
                        ImageUrl = await _imageService.UploadImageAsync(image, "ProductImages"),
                        DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc()
                    };
                }));

                productToCreate.Images = images;
            }

            _context.Products.Add(productToCreate);
            await _context.SaveChangesAsync(cancellationToken);
            if (request.CountInStock.HasValue)
            {
                if (productToCreate.ProductVariations != null)
                {
                    var code = Guid.NewGuid();
                    var wareHouseLogs = new List<WarehouseLogEntity>();
                    foreach (var v in productToCreate.ProductVariations)
                    {
                        var wareHouseLog = new WarehouseLogEntity
                        {
                            DocumentType = DocumentType.Manual,
                            ProductId = productToCreate.Id,
                            ProductVariationId = v.Id,
                            Quantity = v.Quantity ?? 0.0,
                            WarehouseId = request.WarehouseId,
                            Type = WarehouseLogType.In,
                            Code = code,
                            OperationDate = _dateTimeProvider.GetCurrentDateTimeUtc(),
                            SellingPrice = request.SellingPrice
                        };
                        wareHouseLogs.Add(wareHouseLog);
                    }
                    await _context.WarehouseLogs.AddRangeAsync(wareHouseLogs);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                var productSummary =  _context.WarehouseSummaries.Add(new WarehouseSummaryEntity
                {
                    ProductId = productToCreate.Id,
                    Quantity = request.CountInStock??0,
                    WarehouseId=request.WarehouseId,
                });
                await _context.SaveChangesAsync(cancellationToken);

            }
            return Result<Response>.Success(new Response { });
        }
    }
}
