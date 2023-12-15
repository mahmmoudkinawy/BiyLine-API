
namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class CreateShippingCenterFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Name { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.Name)
                .NotEmpty();

            RuleFor(s => s.PickupPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.ReturnCost)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.WeightTo)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.ShippingPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.PricePerExtraKilo)
                .GreaterThanOrEqualTo(0);
        }
    }
    public sealed class Handler: IRequestHandler<Request,Result<Response>>
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
            var traderId = _httpContextAccessor.GetUserById();
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }
            var shippingGovernorateId = _httpContextAccessor.GetValueFromRoute("shippingGovernorateId");
            var shippingGovernorate = await _context.GovernorateShippings
                .Include(g=>g.TraderShippingCompany)
                .FirstOrDefaultAsync(g => g.Id == shippingGovernorateId && g.TraderShippingCompany.StoreId == store.Id);

            if (shippingGovernorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate shipping not exist" });
            }

            var centerShipping = new CenterShippingEntity
            {
                Name = request.Name,
                PickupPrice = request.PickupPrice,
                PricePerExtraKilo = request.PricePerExtraKilo,
                ReturnCost = request.ReturnCost,
                ShippingPrice = request.ShippingPrice,
                WeightTo = request.WeightTo,
                Status = CenterShippingStatusEnum.Active.ToString(),
                GovernorateShippingId = shippingGovernorateId,
            };

            _context.CenterShippings.Add(centerShipping);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }
}
