namespace BiyLineApi.Features.Stores;
public sealed class CreateStoreAddressWithMetadataFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int GovernorateId { get; set; }
        public int RegionId { get; set; }
        public string? Address { get; set; }
        public string? Activity { get; set; }
        public bool? AcceptReturns { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly BiyLineDbContext _context;

        public Validator(BiyLineDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            RuleFor(r => r.GovernorateId)
                .NotEmpty()
                .Must(governorateId => _context.Governments.Any(g => g.Id == governorateId))
                    .WithMessage("Invalid GovernorateId.");

            RuleFor(r => r.RegionId)
                .NotEmpty()
                .Must(regionId => _context.Regions.Any(g => g.Id == regionId))
                    .WithMessage("Invalid RegionId.")
                .Must((r, regionId, context) => ValidateRegionBelongsToGovernorate(r.GovernorateId, regionId))
                    .WithMessage("Region does not belong to the specified Governorate.");

            RuleFor(r => r.Address)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(255);

            RuleFor(r => r.AcceptReturns)
                .NotEmpty()
                .NotNull();

            RuleFor(r => r.Activity)
                .NotEmpty()
                .NotNull()
                .Must(activity => Enum.TryParse<StoreActivityEnum>(activity, out _))
                    .WithMessage($"Please provide a valid activity from StoreActivityEnum. Valid options are: Sectional, Wholesale.");
        }
        private bool ValidateRegionBelongsToGovernorate(int governorateId, int regionId)
            => _context.Regions.Any(r => r.Id == regionId && r.GovernorateId == governorateId);
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

            store.Activity = Enum.TryParse(request.Activity, out StoreActivityEnum activity)
                ? activity
                : default;
            store.AcceptsReturns = request.AcceptReturns;
            store.Address = request.Address;
            store.GovernorateId = request.GovernorateId;
            store.RegionId = request.RegionId;

            store.StoreProfileCompleteness.IsAddressComplete = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }

}
