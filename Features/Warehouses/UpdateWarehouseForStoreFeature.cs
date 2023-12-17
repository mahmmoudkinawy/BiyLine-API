namespace BiyLineApi.Features.Warehouses;
public sealed class UpdateWarehouseForStoreFeature
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

        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
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

            var warehouseId = _httpContextAccessor.GetValueFromRoute("warehouseId");

            if (await _context.Warehouses
                .AnyAsync(w => w.StoreId == store.Id && w.Id == warehouseId && w.Name.ToLower().Equals(request.Name.ToLower()), cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Warehouse with the same name already exists." });
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w =>
                        w.Id == warehouseId &&
                        w.StoreId == store.Id,
                    cancellationToken: cancellationToken);

            if (warehouse == null)
            {
                return Result<Response>.Failure(
                    new List<string> { "Warehouse does not exist." });
            }

            warehouse.WarehouseStatus = request.WarehouseStatus;
            warehouse.Name = request.Name;
            warehouse.ShippingAddress = request.ShippingAddress;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
