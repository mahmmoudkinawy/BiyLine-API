namespace BiyLineApi.Features.Products.Commands;
public sealed class DeleteProductByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
    }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var product = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.StoreId == store.Id,
                    cancellationToken: cancellationToken);

            if (product is null)
            {
                return Result<Response>.Failure(new List<string> { "Product does not exist" });
            }

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}