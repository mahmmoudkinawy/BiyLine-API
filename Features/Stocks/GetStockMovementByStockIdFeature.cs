namespace BiyLineApi.Features.Stocks;
public sealed class GetStockMovementByStockIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int StockId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Trader { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        //public List<ProductResponse> Products { get; set; }
    }

    public sealed class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
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

            var stock = await _context.Stocks
                .Include(s => s.Store)
                .FirstOrDefaultAsync(s => s.Id == request.StockId, cancellationToken: cancellationToken);

            if (stock is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen stock." });
            }

            return Result<Response>.Success(new Response
            {
                Id = stock.Id,
                Date = stock.Created,
                From = stock.DestinationWarehouse.Name,
                To = stock.SourceWarehouse.Name,
                Trader = stock.Store.Username
            });
        }
    }
}
