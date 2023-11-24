namespace BiyLineApi.Features.Basket;
public class CurrentUserBasketFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalPrice { get; set; }

        public ICollection<BasketItemResponse> BasketItems { get; set; }
    }

    public sealed class BasketItemResponse
    {
        public int? Quantity { get; set; }
        public int? ProductId { get; set; }
        public decimal Price { get; set; }
        public string? Name { get; set; }
        public string? ImageSrc { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context,
            IMapper mapper,
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _dateTimeProvider = dateTimeProvider ??
                throw new ArgumentNullException(nameof(dateTimeProvider));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User?.GetUserById();

            var basket = await _context.Baskets
                .IgnoreQueryFilters()
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken: cancellationToken);

            basket ??= new BasketEntity
            {
                CreatedAt = _dateTimeProvider.GetCurrentDateTimeUtc(),
                UserId = userId
            };

            return new Response
            {
                Id = basket.Id,
                BasketItems = basket.BasketItems.Select(x => new BasketItemResponse
                {
                    ImageSrc = x.ImageSrc,
                    Name = x.Name,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList(),
                TotalPrice = basket.TotalPrice,
            };
        }
    }
}
