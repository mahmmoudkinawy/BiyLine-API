namespace BiyLineApi.Features.Inventories;
public sealed class UpdateInventoryFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? CodeNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CodeNumber)
                .NotEmpty()
                .MaximumLength(255);
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

            var warehouseId = _httpContextAccessor.GetValueFromRoute("warehouseId");

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.StoreId == store.Id && w.Id == warehouseId,
                    cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not this warehouse." });
            }

            if (await _context.Inventories
                .AnyAsync(w => w.WarehouseId == warehouseId && w.CodeNumber.Equals(request.CodeNumber), cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Inventory with the same name already exists in this Inventory." });
            }

            var inventoryId = _httpContextAccessor.GetValueFromRoute("inventoryId");

            var inventoryFromDb = await _context.Inventories
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.Id == inventoryId,
                    cancellationToken: cancellationToken);

            if (inventoryFromDb == null)
            {
                return Result<Response>.Failure(new List<string> { "Inventory does not exist." });
            }

            inventoryFromDb.WarehouseId = warehouseId;
            inventoryFromDb.CodeNumber = request.CodeNumber;
            inventoryFromDb.Created = request.Date;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
