namespace BiyLineApi.Features.Coupons;
public sealed class CreateCouponFeature
{
    public sealed class Request : IRequest
    {
        public string? Code { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? CommissionRate { get;  set; }
        public decimal? DiscountPercentage { get;  set; }
        public string? Name { get; internal set; }
        public List<int> CategoriesIds { get; set; } = new List<int>();
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly BiyLineDbContext _context;

        public Validator(BiyLineDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            RuleFor(r => r.Code)
                .NotEmpty()
                .MaximumLength(300)
                .Must(code => !code.Contains(" "))
                    .WithMessage("Coupon code must not contain spaces.")
                .Must(code => !CouponExists(code))
                    .WithMessage("Coupon code already exists and active.");

            RuleFor(r => r.DiscountAmount)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.StartDate)
                .NotEmpty()
                .NotNull()
                .LessThanOrEqualTo(r => r.EndDate)
                .WithMessage("Start date must be less than or equal to the end date.");

            RuleFor(r => r.EndDate)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("End date must be greater than or equal to the current UTC time.");
        }

        private bool CouponExists(string code)
        {
            var couponExists = _context.Coupons.Any(c => c.Code == code);

            return couponExists;
        }
    }

    public sealed class Handler : IRequestHandler<Request>
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

        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var coupon = new CouponEntity
            {
                StoreId = store.Id,
                Code = request.Code,
                DiscountAmount = request.DiscountAmount.Value,
                StartDate = request.StartDate,
                EndDate = request.EndDate,CommissionRate = request.CommissionRate,
                DiscountPercentage = request.DiscountPercentage,
                Name = request.Name
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync(cancellationToken);
            foreach (var item in request.CategoriesIds)
            {
                _context.CouponCategory.Add(new CouponCategory
                {
                    CategoryId = item,
                    CouponId = coupon.Id
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
