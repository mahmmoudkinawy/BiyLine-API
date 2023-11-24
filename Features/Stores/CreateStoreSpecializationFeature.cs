namespace BiyLineApi.Features.Stores;
public sealed class CreateStoreSpecializationFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public List<SubSpecializationRequest>? Specializations { get; set; }
    }

    public sealed class SubSpecializationRequest
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(150);

            RuleFor(r => r.Specializations)
                .Must(subSpecialization => subSpecialization == null || subSpecialization.Count <= 5)
                .WithMessage("The number of specializations must be less than or equal to 5.")
                .Must(subSpecialization => subSpecialization == null || subSpecialization.All(IsValidImageRequest))
                .WithMessage("Only PNG, JPG, and JPEG file extensions are allowed for images.")
                .When(r => r.Specializations != null);

            RuleForEach(r => r.Specializations)
                .ChildRules(subSpecialization =>
                {
                    subSpecialization.RuleFor(i => i.Name)
                        .NotEmpty()
                        .MinimumLength(2)
                        .MaximumLength(150);

                    subSpecialization.RuleFor(i => i.Image)
                        .NotNull()
                        .Must(IsSupportedImageExtension)
                        .WithMessage("Only PNG, JPG, and JPEG file extensions are allowed for images.")
                        .Must(image => image != null && image.Length <= 2 * 1024 * 1024)
                        .WithMessage("Image size must not exceed 2 MB.")
                        .When(i => i != null);
                });
        }

        private static bool IsValidImageRequest(SubSpecializationRequest subSpecializationRequest)
        {
            if (subSpecializationRequest == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(subSpecializationRequest.Name) && subSpecializationRequest.Image != null;
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

            var specialization = new SpecializationEntity
            {
                Name = request.Name
            };

            if (request.Specializations != null)
            {
                var subSpecializations = await Task.WhenAll(
                request.Specializations.Select(async subSpec => new SubSpecializationEntity
                {
                    Name = subSpec.Name,
                    SubSpecializationImage = new ImageEntity
                    {
                        DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        FileName = subSpec.Image.FileName,
                        ImageMimeType = subSpec.Image.ContentType,
                        ImageUrl = await _imageService.UploadImageAsync(subSpec.Image, "SpecializationImages"),
                        Type = "SubSpecializationImage"
                    }
                }));

                specialization.SubSpecializations = subSpecializations;
            }

            store.Specializations.Add(specialization);

            store.StoreProfileCompleteness.IsSpecializationsComplete = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
