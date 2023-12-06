namespace BiyLineApi.Features.Warehouses;
public sealed class GetSummaryWarehouseFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Predicate { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int WarehouseId { get; set; }
    }

    public sealed class Response
    {
        // Missing properties
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.StoreId == store.Id, cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current store does not owen this warehouse." });
            }

            var query = _context.Warehouses
                .Where(p => p.StoreId == store.Id && p.Id == warehouse.Id)
                .SelectMany(w => w.Products)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(p => p.ProductTranslations.Any(pt => pt.Name.Contains(request.Predicate)));
            }

            if (request.From != null && request.To != null)
            {
                var fromDate = request.From.Value.Date;
                var toDate = request.To.Value.Date;

                query = query.Where(w => w.Warehouse.Created.Value.Date >= fromDate
                    && w.Warehouse.Created.Value.Date <= toDate);
            }

            var result = query.Select(wp => new Response
            {

            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
