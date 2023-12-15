namespace BiyLineApi.Features.TraderShippingCompany;
public sealed class ApplyTheGovernoratePropertiesOnAllCentersFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ShippingGovernorateId { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.ShippingGovernorateId)
                .GreaterThan(0);
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

            var governorateShipping = await _context.GovernorateShippings
                .Include(g => g.TraderShippingCompany)
                .Include(g => g.CenterShippings)
                .FirstOrDefaultAsync(g => g.Id == request.ShippingGovernorateId && g.TraderShippingCompany.StoreId == store.Id);

            if (governorateShipping == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate not exist" });
            }

            foreach (var shippingCenter in governorateShipping.CenterShippings)
            {
                shippingCenter.ReturnCost = governorateShipping.ReturnCost;
                shippingCenter.ShippingPrice = governorateShipping.ShippingPrice;
                shippingCenter.PickupPrice = governorateShipping.PickupPrice;
                shippingCenter.WeightTo = governorateShipping.WeightTo;
                shippingCenter.PricePerExtraKilo = governorateShipping.PricePerExtraKilo;
            }

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
