namespace BiyLineApi.Features.Stocks;
public sealed class GetStocksMovementsFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime? Date { get; set; }
        public int? SourceWarehouseId { get; set; }
        public int? DestinationWarehouseId { get; set; }
        public string? InvoiceNumber { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
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
        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            var query = _context.Stocks
                .OrderBy(s => s.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.InvoiceNumber))
            {
                query = query.Where(s => s.InvoiceNumber.Contains(request.InvoiceNumber));
            }

            if (request.SourceWarehouseId != null && request.SourceWarehouseId > 0)
            {
                query = query.Where(s => s.SourceWarehouseId == request.SourceWarehouseId.Value);
            }

            if (request.DestinationWarehouseId != null && request.DestinationWarehouseId > 0)
            {
                query = query.Where(s => s.DestinationWarehouseId == request.DestinationWarehouseId.Value);
            }

            if (request.Date != null)
            {
                query = query.Where(s => s.Created.Date == request.Date.Value);
            }

            var result = query.Select(s => new Response
            {
                Id = s.Id,
                Date = s.Created,
                From = s.SourceWarehouse.Name,
                To = s.DestinationWarehouse.Name
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
