namespace BiyLineApi.Features.Warehouses;
public sealed class DeleteWarehouseForStoreFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int WarehouseId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.WarehouseId)
                .GreaterThan(0);
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

            var warehouseToDelete = await _context.Warehouses
                .FirstOrDefaultAsync(w =>
                    w.Id == request.WarehouseId && w.StoreId == store.Id, cancellationToken: cancellationToken);

            if (warehouseToDelete is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen this warehouse." });
            }

            _context.Warehouses.Remove(warehouseToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
