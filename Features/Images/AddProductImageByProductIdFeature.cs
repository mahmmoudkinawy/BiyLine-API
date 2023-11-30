namespace BiyLineApi.Features.Images;
public sealed class AddProductImageByProductIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public IFormFile? Image { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Image)
                .Must(IsValidImage)
                    .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
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

            var productId = _httpContextAccessor.GetValueFromRoute("productId");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.StoreId == store.Id && p.Id == productId,
                    cancellationToken: cancellationToken);

            if (product is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this product" });
            }

            var imageUrl = await _imageService.UploadImageAsync(request.Image, "ProductImages");

            var imageToCreate = new ImageEntity
            {
                FileName = request.Image.FileName,
                ImageMimeType = request.Image.ContentType,
                DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                StoreId = store.Id,
                ImageUrl = imageUrl,
                Type = "ProductImage",
                ProductId = productId
            };

            _context.Images.Add(imageToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
