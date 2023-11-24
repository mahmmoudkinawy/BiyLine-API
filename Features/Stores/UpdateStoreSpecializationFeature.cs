namespace BiyLineApi.Features.Stores;
public sealed class UpdateStoreSpecializationFeature
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
                .NotEmpty()
                .NotNull()
                .Must(subSpecialization => subSpecialization == null || subSpecialization.Count <= 5)
                    .WithMessage("The number of images must be less than or equal to 5.")
                .Must(subSpecialization => subSpecialization == null || subSpecialization.All(IsValidImageRequest))
                    .WithMessage("Only PNG, JPG, and JPEG file extensions are allowed for images.");

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
                        .WithMessage("Image size must not exceed 2 MB.");
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
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store" });
            }

            store.Specializations = await _context.Specializations
                .Include(s => s.SubSpecializations)
                .Where(s => s.StoreId == store.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            var subSpecializationId = _httpContextAccessor.GetValueFromRoute("subSpecializationId");

            var subSpecialization = store.Specializations
                .SelectMany(s => s.SubSpecializations)
                .FirstOrDefault(subSpec => subSpec.Id == subSpecializationId);

            if (subSpecialization is null)
            {
                return Result<Response>.Failure(
                    new List<string> { "Current user does not owen sub specialization with the given id" });
            }

            // Logic is not completed yet

            //store.Specializations.Add(new SpecializationEntity
            //{
            //    Name = request.Name,
            //    SubSpecializations = subSpecializations.ToList()
            //});

            //store.StoreProfileCompleteness.IsSpecializationsComplete = true;

            //await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
