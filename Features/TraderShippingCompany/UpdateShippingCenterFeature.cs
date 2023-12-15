namespace BiyLineApi.Features.TraderShippingCompany;
public sealed class UpdateShippingCenterFeature
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

            var shippingCenterId = _httpContextAccessor.GetValueFromRoute("shippingCenterId");

            var shippingCenter = await _context.CenterShippings
                .Include(c => c.GovernorateShipping)
                .ThenInclude(c => c.TraderShippingCompany)
                .FirstOrDefaultAsync(c => c.Id == shippingCenterId && c.GovernorateShipping.TraderShippingCompany.StoreId == store.Id);

            if (shippingCenter == null)
            {
                return Result<Response>.Failure(new List<string> { "this center not exist for this store" });
            }

            if (_context.CenterShippings
                .Include(c => c.GovernorateShipping)
                .ThenInclude(c => c.TraderShippingCompany)
                .Any(cs => cs.Name == request.Name && cs.GovernorateShipping.TraderShippingCompany.StoreId == store.Id && cs.Id != shippingCenterId))
            {
                return Result<Response>.BadRequest(new List<string> { "A center company with the same name already exists for this store" });
            }

            shippingCenter.Name = request.Name;
            shippingCenter.ReturnCost = request.ReturnCost;
            shippingCenter.ShippingPrice = request.ShippingPrice;
            shippingCenter.PickupPrice = request.PickupPrice;
            shippingCenter.WeightTo = request.WeightTo;
            shippingCenter.PricePerExtraKilo = request.PricePerExtraKilo;

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
