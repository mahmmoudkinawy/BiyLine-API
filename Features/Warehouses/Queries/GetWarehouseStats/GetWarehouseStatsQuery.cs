namespace BiyLineApi.Features.Warehouses.Queries.GetWarehouseStats
{
    public class GetWarehouseStatsQuery
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int WarehouseId { get; set; }
        }

        public sealed class Response
        {
            public int ProductCount { get; set; }
            public decimal ProductCostValue { get; set; }
            public decimal ProductSellingValue { get; set; }
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

                var warehouse = await _context.Warehouses.Include(w => w.Logs).ThenInclude(l => l.Product)
                    .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.StoreId == store.Id,
                        cancellationToken: cancellationToken);

                if (warehouse == null)
                {
                    return Result<Response>.Failure(new List<string> { "Warehouse does not exists for this store." });
                }
                var productCount = warehouse.Logs.Select(l => l.ProductId).Distinct().Count();
                var productSellingValue = warehouse.Logs.Select(l => l.Product.SellingPrice).Sum();
                var productCostValue = warehouse.Logs.Select(l => l.Product.OriginalPrice).Sum();
                var result = new Response
                {
                    ProductCostValue = productCostValue??0,
                    ProductCount = productCount,
                    ProductSellingValue = productSellingValue??0
                };

                return Result<Response>.Success(result);
            }
        }
    }
}
