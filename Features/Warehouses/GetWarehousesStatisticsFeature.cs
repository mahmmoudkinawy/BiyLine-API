namespace BiyLineApi.Features.Warehouses;
public sealed class GetWarehousesStatisticsFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int WarehouseId { get; set; }
    }

    public sealed class Response
    {
        public int ProductsCount { get; set; }
        public decimal? TotalProductPrices { get; set; }
        public int OrdersCount { get; set; }
        public decimal ReturnedValue { get; set; }
        public int ReadyForDeliveryCount { get; set; }
    }

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
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
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
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.StoreId == store.Id,
                    cancellationToken: cancellationToken);

            if (warehouse == null)
            {
                return Result<Response>.Failure(new List<string> { "Warehouse does not exists for this store." });
            }

            var result = new Response
            {
                // Following 3 must resolved in the future
                OrdersCount = new Random().Next(0, 100),
                ReadyForDeliveryCount = new Random().Next(0, 100),
                ReturnedValue = new Random().Next(0, 100),
                ProductsCount = await _context.Warehouses
                    .IgnoreQueryFilters()
                    .Include(w => w.Products)
                    .SelectMany(w => w.Products)
                    .Where(w => w.WarehouseId == request.WarehouseId)
                    .CountAsync(cancellationToken: cancellationToken),
                TotalProductPrices = await _context.Warehouses
                    .IgnoreQueryFilters()
                    .Include(w => w.Products)
                    .SelectMany(w => w.Products)
                    .Where(w => w.WarehouseId == request.WarehouseId)
                    .SumAsync(w => w.SellingPrice, cancellationToken: cancellationToken)
            };

            return Result<Response>.Success(result);
        }
    }
}
