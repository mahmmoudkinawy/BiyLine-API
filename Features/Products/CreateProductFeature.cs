 namespace BiyLineApi.Features.Products;
public sealed class CreateProductFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
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
        public List<ColorQuantityRequest>? Colors { get; set; }
        public List<SizeQuantityRequest>? Sizes { get; set; }
    }

    public sealed class ColorQuantityRequest
    {
        public string? Color { get; set; }
        public int? Quantity { get; set; }
    }

    public sealed class SizeQuantityRequest
    {
        public string? Size { get; set; }
        public int? Quantity { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly BiyLineDbContext _context;

        public Validator(BiyLineDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            RuleFor(r => r.CategoryId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .Must((req, _) => _context.Categories.Any(c => c.Id == req.CategoryId))
                    .WithMessage("Category with the given Id does not exist");

            RuleFor(r => r.WarehouseId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.SubcategoryId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .Must((req, _) =>
                        _context.Subcategories.Any(c => c.Id == req.SubcategoryId) &&
                        _context.Categories.Any(c => c.Id == req.CategoryId && c.Subcategories.Any(sc => sc.Id == req.SubcategoryId)))
                    .WithMessage("SubcategoryId with the given Id does not exist");

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

            When(req => req.Colors != null && req.Colors.Any(), () =>
            {
                RuleFor(r => r.Colors)
                    .NotEmpty()
                    .NotNull()
                    .Must(colors => colors.Count <= 10)
                        .WithMessage("Maximum of 10 colors allowed")
                    .Must(colors => colors.Distinct().Count() == colors.Count)
                        .WithMessage("Colors must be unique");

                RuleForEach(req => req.Colors)
                    .SetValidator(new ColorQuantityRequestValidator());
            });

            When(req => req.Sizes != null && req.Sizes.Any(), () =>
            {
                RuleFor(r => r.Sizes)
                    .NotEmpty()
                    .NotNull()
                    .Must(sizes => sizes.Count <= 10)
                        .WithMessage("Maximum of 10 sizes allowed")
                    .Must(sizes => sizes.Distinct().Count() == sizes.Count)
                        .WithMessage("Sizes must be unique");

                RuleForEach(req => req.Sizes)
                    .SetValidator(new SizeQuantityRequestValidator());
            });
        }

        public sealed class ColorQuantityRequestValidator : AbstractValidator<ColorQuantityRequest>
        {
            public ColorQuantityRequestValidator()
            {
                RuleFor(req => req.Color)
                    .NotEmpty()
                    .NotNull();

                RuleFor(req => req.Quantity)
                    .InclusiveBetween(0, int.MaxValue)
                    .When(req => req.Color != null);
            }
        }

        public class SizeQuantityRequestValidator : AbstractValidator<SizeQuantityRequest>
        {
            public SizeQuantityRequestValidator()
            {
                RuleFor(req => req.Size)
                    .NotEmpty()
                    .NotNull();

                RuleFor(req => req.Quantity)
                    .InclusiveBetween(0, int.MaxValue)
                    .When(req => req.Size != null);
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
                CategoryId = request.CategoryId,
                CodeNumber = request.CodeNumber,
                ThresholdReached = request.ThresholdReached,
                Colors = request.Colors != null ?
                        request.Colors.Select(color => new ProductColorEntity
                        {
                            Color = color.Color,
                            Quantity = color.Quantity
                        }).ToList()
                    : null,
                Sizes = request.Sizes != null ?
                        request.Sizes.Select(size => new ProductSizeEntity
                        {
                            Size = size.Size,
                            Quantity = size.Quantity
                        }).ToList()
                    : null,
                CountInStock = request.CountInStock,
                IsInStock = request.IsInStock,
                SellingPrice = request.SellingPrice,
                OriginalPrice = request.OriginalPrice,
                Vat = request.Vat,
                SubcategoryId = request.SubcategoryId,
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
                }
            };

            if (request.Images != null)
            {
                var images = await Task.WhenAll(request.Images?.Select(async image =>
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

            return Result<Response>.Success(new Response { });
        }
    }
}
