namespace BiyLineApi.Features.Warehouses;
public sealed class CreateWarehouseForStoreFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? ShippingAddress { get; set; }
        public string? WarehouseStatus { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(255);

            RuleFor(r => r.ShippingAddress)
                .NotEmpty()
                .NotNull()
                .MaximumLength(500);

            RuleFor(r => r.WarehouseStatus)
                .NotEmpty()
                .NotNull()
                .Must(activity => Enum.TryParse<WarehouseStatusEnum>(activity, out _))
                    .WithMessage($"Please provide a valid warehouse from WarehouseStatusEnum. Valid options are: Active, Main.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            if (await _context.Warehouses
                .AnyAsync(w => w.StoreId == store.Id && w.Name.ToLower().Equals(request.Name.ToLower()), cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Warehouse with the same name already exists." });
            }

            var warehouseToCreate = new WarehouseEntity
            {
                Name = request.Name,
                StoreId = store.Id,
                ShippingAddress = request.ShippingAddress,
                WarehouseStatus = request.WarehouseStatus
            };

            _context.Warehouses.Add(warehouseToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
