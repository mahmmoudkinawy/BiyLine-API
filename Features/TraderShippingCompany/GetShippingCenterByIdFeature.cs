namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class GetShippingCenterByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ShippingCenterId { get; set; }
    }
    public sealed class Response
    {
        public int Id { get; set; }
        public string ShippingCenterName { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
        public string Status { get; set; }
        public string ShippingGovernorateName { get; set; }

    }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.ShippingCenterId)
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
            var store = await _context.Stores
                .Include(s => s.TraderShippingCompanies)
                .FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }

            var centerShipping = await _context.CenterShippings
                .Include(g => g.GovernorateShipping)
                   .ThenInclude(g => g.TraderShippingCompany)
                      .ThenInclude(t=>t.Store)
                .Include(g => g.GovernorateShipping)
                   .ThenInclude(g=>g.Governorate)
                .FirstOrDefaultAsync(s => s.Id == request.ShippingCenterId && s.GovernorateShipping.TraderShippingCompany.StoreId == store.Id);

            if (centerShipping == null)
            {
                return Result<Response>.Failure(new List<string> { "this center shipping not exist for this store" });
            }

            var response = new Response
            {
                Id = request.ShippingCenterId,
                ShippingCenterName = centerShipping.Name,
                PickupPrice = centerShipping.PickupPrice,
                ShippingPrice = centerShipping.ShippingPrice,
                PricePerExtraKilo = centerShipping.PricePerExtraKilo,
                ReturnCost = centerShipping.ReturnCost,
                WeightTo = centerShipping.WeightTo,
                Status = centerShipping.Status,
                ShippingGovernorateName = centerShipping.GovernorateShipping.Governorate.Name
            };

            return Result<Response>.Success(response);
        }
    }
}
