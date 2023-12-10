namespace BiyLineApi.Features.StockTrackers;
public sealed class CreateStockTrackerForWarehouseFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string StockTrackerNumber { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.StockTrackerNumber)
                .NotEmpty()
                .MaximumLength(500);
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

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.StoreId == store.Id && w.Id == warehouseId,
                    cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen warehouse." });
            }

            var stockTrackerToCreate = new StockTrackerEntity
            {
                Date = request.Date,
                StockTrackerNumber = request.StockTrackerNumber,
                StoreId = store.Id,
                WarehouseId = warehouseId
            };

            _context.StockTrackers.Add(stockTrackerToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }

}
