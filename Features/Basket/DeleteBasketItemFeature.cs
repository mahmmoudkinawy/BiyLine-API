namespace BiyLineApi.Features.Basket;
public sealed class DeleteBasketItemFeature
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

        public Handler(BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User?.GetUserById();

            var basket = await _context.Baskets
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken: cancellationToken);

            if (basket is null)
            {
                return Result<Response>.Failure("Product does not exist");
            }

            var productInBasket = basket.BasketItems.FirstOrDefault(bi => bi.ProductId == request.ProductId);

            if (productInBasket is null)
            {
                return Result<Response>.Failure("Product with this id does not exist");
            }

            _context.BasketItems.Remove(productInBasket);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
