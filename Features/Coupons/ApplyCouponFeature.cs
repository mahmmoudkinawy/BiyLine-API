using BiyLineApi.Extensions;

namespace BiyLineApi.Features.Coupons;
public sealed class ApplyCouponFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? CouponCode { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceBeforeDiscount { get; set; }

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

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ??
                throw new ArgumentNullException(nameof(dateTimeProvider));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User?.GetUserById();

            var basket = await _context.Baskets
                .IgnoreQueryFilters()
                .Include(b => b.BasketItems).ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken: cancellationToken);

            if (basket is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    // Replace with localization
                    "basket is empty!."
                });
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c =>
                    c.Code == request.CouponCode && c.EndDate > _dateTimeProvider.GetCurrentDateTimeUtc(), cancellationToken: cancellationToken);

            if (coupon is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    // Replace with localization
                    "Coupon does not found or expired."
                });
            }
            var totalBeforeDiscount = basket.BasketItems.Sum(bi => bi.Price * bi.Quantity.Value);

            basket.TotalPrice -= coupon.DiscountAmount;

            CouponUsageEntity couponUsage = new CouponUsageEntity
            {
                CouponId = coupon.Id,
                ItemCount = basket.BasketItems.Count,
                Price = basket.TotalPrice * coupon.CommissionRate.Value,
                UserId = userId.Value
            };
            _context.CouponsUsages.Add(couponUsage);
            await _context.SaveChangesAsync(cancellationToken);

            //var totalBeforeDiscount = basket.BasketItems.Sum(bi => bi.Price * bi.Quantity.Value);

            return Result<Response>.Success(new Response
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
                TotalPriceBeforeDiscount = totalBeforeDiscount,
                TotalPrice = basket.TotalPrice,
            });
        }
    }
}
