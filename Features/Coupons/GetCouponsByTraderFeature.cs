
namespace BiyLineApi.Features.Coupons;
public sealed class GetCouponsByTraderFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Code { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CouponStatus IsActive { get; set; }
        public int UsageCount { get; set; } = new Random().Next(1, 500); // will be replaced
        public string? Name { get; internal set; }
        public decimal? CommissionRate { get; internal set; }
        public decimal? DiscountPercentage { get; internal set; }
        public List<CategoryEntity> categories { get; internal set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.StartDate)
                .NotEmpty()
                .NotNull()
                .LessThanOrEqualTo(r => r.EndDate)
                .WithMessage("Start date must be less than or equal to the end date.");

            RuleFor(r => r.EndDate)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddMonths(1))
                .WithMessage("End date must be greater than or equal to the current UTC time.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var query = _context.Coupons
                .Where(c => c.StoreId == store.Id)
                .OrderByDescending(c => c.EndDate)
                    .ThenByDescending(c => c.StartDate)
                .AsQueryable();

            if (request.StartDate != null && request.EndDate != null)
            {
                query = query.Where(c => c.StartDate >= request.StartDate && c.EndDate <= request.EndDate);
            }

            var result = query.Select(coupon => new Response
            {
                Id = coupon.Id,
                Code = coupon.Code,
                StartDate = coupon.StartDate.Value,
                DiscountAmount = coupon.DiscountAmount,
                EndDate = coupon.EndDate.Value,
                IsActive = coupon.Status,
                UsageCount = coupon.Usage.Count,
                Name = coupon.Name,
                CommissionRate = coupon.CommissionRate,
                DiscountPercentage = coupon.DiscountPercentage,
                categories = _context.CouponCategory
                .Where(cc => cc.CouponId == coupon.Id)
                .Select(cc => cc.Category)
                .ToList()
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}