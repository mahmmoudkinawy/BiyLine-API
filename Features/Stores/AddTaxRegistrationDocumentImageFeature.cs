namespace BiyLineApi.Features.Stores;
public sealed class AddTaxRegistrationDocumentImageFeature
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
                .NotEmpty()
                .NotNull()
                .Must(IsSupportedImageExtension)
                    .WithMessage("Only PNG, JPG, and JPEG file extensions are allowed for images.")
                .Must(image => image != null && image.Length <= 2 * 1024 * 1024)
                    .WithMessage("Image size must not exceed 2 MB.");
        }

        private static bool IsSupportedImageExtension(IFormFile image)
        {
            if (image == null)
            {
                return false;
            }

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(image.FileName);

            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }
    }

    public sealed class Handler : IRequestHandler<Request,Result<Response>>
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
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ??
                throw new ArgumentNullException(nameof(dateTimeProvider));
            _imageService = imageService ??
                throw new ArgumentNullException(nameof(imageService));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var store = await _context.Stores
                .Include(s => s.StoreProfileCompleteness)
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store" });
            }

            var imageUrl = await _imageService.UploadImageAsync(request.Image, "TaxDocumentImages");

            store.TaxRegistrationDocumentImage = new ImageEntity
            {
                StoreId = store.Id,
                DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                FileName = request.Image.FileName,
                ImageMimeType = request.Image.ContentType,
                ImageUrl = imageUrl,
                Type = "TaxRegistrationDocumentImage"
            };

            store.StoreProfileCompleteness.IsTaxImageComplete = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
