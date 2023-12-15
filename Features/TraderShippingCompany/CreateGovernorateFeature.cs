namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class CreateGovernorateFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
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
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

            var governorateId = _httpContextAccessor.GetValueFromRoute("governorateId");
            var governorate = await _context.Governments.FirstOrDefaultAsync(g => g.Id == governorateId);

            if (governorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate not exist" });
            }
            var traderShippingCompanyId = _httpContextAccessor.GetValueFromRoute("traderShippingCompanyId");

            var traderShippingCompany = await _context.TraderShippingCompanies.FirstOrDefaultAsync(s => s.Id == traderShippingCompanyId && s.StoreId == store.Id);

            if (traderShippingCompany == null)
            {
                return Result<Response>.Failure(new List<string> { "this shipping company not exist" });
            }

            var shippingGovernorate = new GovernorateShippingEntity
            {
                ShippingPrice = request.ShippingPrice,
                PickupPrice = request.PickupPrice,
                ReturnCost = request.ReturnCost,
                PricePerExtraKilo = request.PricePerExtraKilo,
                WeightTo = request.WeightTo,
                GovernorateId = governorateId,
                TraderShippingCompanyId = traderShippingCompanyId,
            };

            _context.GovernorateShippings.Add(shippingGovernorate);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
