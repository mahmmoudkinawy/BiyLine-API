namespace BiyLineApi.Features.Products;
public sealed class UpdateProductFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public string? ArabicName { get; set; }
        public string? EnglishName { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? CodeNumber { get; set; }
        public decimal? Vat { get; set; }
        public int? ThresholdReached { get; set; }
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

            var productId = _httpContextAccessor.GetValueFromRoute("productId");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.StoreId == store.Id && p.Id == productId,
                    cancellationToken: cancellationToken);

            if (product is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this product" });
            }

            product.CategoryId = request.CategoryId;
            product.CodeNumber = request.CodeNumber;
            product.ThresholdReached = request.ThresholdReached;
            product.CountInStock = request.CountInStock;
            product.IsInStock = request.IsInStock;
            product.SellingPrice = request.SellingPrice;
            product.OriginalPrice = request.OriginalPrice;
            product.Vat = request.Vat;
            product.SubcategoryId = request.SubcategoryId;

            var arTranslation = product.ProductTranslations.FirstOrDefault(t => t.Language == "ar");
            if (arTranslation != null)
            {
                arTranslation.Name = request.ArabicName;
                arTranslation.Description = request.ArabicDescription;
                arTranslation.GeneralOverview = request.GeneralOverview;
            }
            else
            {
                product.ProductTranslations.Add(new ProductTranslationEntity
                {
                    Language = "ar",
                    Name = request.ArabicName,
                    Description = request.ArabicDescription,
                    GeneralOverview = request.GeneralOverview
                });
            }

            var enTranslation = product.ProductTranslations.FirstOrDefault(t => t.Language == "en");
            if (enTranslation != null)
            {
                enTranslation.Name = request.EnglishName;
                enTranslation.Description = request.EnglishDescription;
                enTranslation.GeneralOverview = request.GeneralOverview;
            }
            else
            {
                product.ProductTranslations.Add(new ProductTranslationEntity
                {
                    Language = "en",
                    Name = request.EnglishName,
                    Description = request.EnglishDescription,
                    GeneralOverview = request.GeneralOverview
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
