namespace BiyLineApi.Features.Stores;
public sealed class CreateStoreDetailsFeature
{
    public sealed class Request : IRequest
    {
        public int? CountryId { get; set; }
        public string? ArabicStoreName { get; set; }
        public string? EnglishStoreName { get; set; }
        public string? StoreUsername { get; set; }
        public List<int>? Categories { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly BiyLineDbContext _context;

        public Validator(BiyLineDbContext context)
        {
            _context = context;

            RuleFor(r => r.CountryId)
                .NotEmpty()
                .Must((countryId) => _context.Countries.Any(c => c.Id == countryId))
                    .WithMessage("Country with this Id does not exist.");

            RuleFor(r => r.ArabicStoreName)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(255);

            RuleFor(r => r.EnglishStoreName)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(255);

            RuleFor(r => r.StoreUsername)
                .NotEmpty()
                .Must((username) => !_context.Stores.Any(c => c.Username == username))
                    .WithMessage("This username already exists.");

            When(r => r.Categories.Any(), () =>
            {
                RuleFor(r => r.Categories)
                    .Must(BeDistinct)
                        .WithMessage("Must selected a distinct categories.")
                    .Must(categories => _context.Categories.Any(c => categories.Contains(c.Id)))
                        .WithMessage("One or more category IDs do not exist in the database.");
            });
        }

        private bool BeDistinct(List<int>? categories) => categories == null || categories.Distinct().Count() == categories.Count;
    }

    public sealed class Handler : IRequestHandler<Request>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var storeToCreate = new StoreEntity
            {
                OwnerId = userId,
                ArabicName = request.ArabicStoreName,
                EnglishName = request.EnglishStoreName,
                Username = request.StoreUsername,
                CountryId = request.CountryId,
                StoreProfileCompleteness = new StoreProfileCompletenessEntity
                {
                    IsDetailsComplete = true,
                    UserId = userId
                }
            };

            storeToCreate.StoreCategories = request.Categories.Select(categoryId => new StoreCategoryEntity
            {
                CategoryId = categoryId,
                StoreId = storeToCreate.Id
            }).ToList();

            _context.Stores.Add(storeToCreate);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
