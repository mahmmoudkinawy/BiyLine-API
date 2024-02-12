using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Features.Coupons;
public sealed class UpdateCouponFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<int> CategoriesId { get; set; } = new List<int>();

        public decimal? DiscountPercentage { get; set; }
        public decimal? CommissionRate { get; set; }
    }
    public sealed class Response { }
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

    public sealed class Handler : IRequestHandler<Request,Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);
            if (coupon == null)
            {
                return Result<Response>.Failure(new List<string> {
                    "Cupon Not Found"
                });
            }
      
                coupon.StoreId = store.Id;
                coupon.Code = request.Code;
                coupon.DiscountAmount = request.DiscountAmount.Value;
                coupon.StartDate = request.StartDate;
                coupon.EndDate = request.EndDate;
                coupon.CommissionRate = request.CommissionRate;
                coupon.DiscountPercentage = request.DiscountPercentage;
          

            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync(cancellationToken);
            foreach (var item in request.CategoriesId)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == item);
                if (category != null)
                {
                    if(!_context.CouponCategory.Any(cc => cc.CouponId == coupon.Id && cc.CategoryId == category.Id))
                    {
                        await _context.CouponCategory.AddAsync(new CouponCategory
                        {
                            CategoryId = category.Id,
                            CouponId = coupon.Id
                        });
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result<Response>.Success(new Response { });
        }
    }
}
