namespace BiyLineApi.Features.Stocks;
public sealed class DeleteStockMovementByStockIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int StockId { get; set; }
    }

    public sealed class Response { }

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

            var stock = await _context.Stocks
                .FindAsync(new object?[] { request.StockId }, cancellationToken: cancellationToken);

            if (stock is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen stock." });
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
