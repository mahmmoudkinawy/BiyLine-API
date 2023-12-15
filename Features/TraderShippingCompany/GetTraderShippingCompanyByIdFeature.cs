namespace BiyLineApi.Features.TraderShippingCompany;
public sealed class GetTraderShippingCompanyByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int TraderShippingCompanyId { get; set; }
    }
    public sealed class Response
    {
        public string TraderShippingCompanyName { get; set; }
        public List<shippingGovernorateResponse> ShippingGovernorates { get; set; } = new List<shippingGovernorateResponse>();
    }
    public sealed class shippingGovernorateResponse
    {
        public int Id { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
        public string GovernorateName { get; set; }

    }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.TraderShippingCompanyId)
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

            var traderShippingCompany = await _context.TraderShippingCompanies
                .Include(s=>s.GovernorateShippings)
                .ThenInclude(gs=>gs.Governorate)
                .FirstOrDefaultAsync(s => s.Id == request.TraderShippingCompanyId && s.StoreId == store.Id);

            if (traderShippingCompany == null)
            {
                return Result<Response>.Failure(new List<string> { "this shipping company does not exist" });
            }

            var response = new Response
            {
                TraderShippingCompanyName = traderShippingCompany.Name,
                ShippingGovernorates = traderShippingCompany.GovernorateShippings.Select(gs => new shippingGovernorateResponse
                {
                    Id = gs.Id,
                    PickupPrice = gs.PickupPrice,
                    PricePerExtraKilo = gs.PricePerExtraKilo,
                    ReturnCost = gs.ReturnCost,
                    ShippingPrice = gs.ShippingPrice,
                    WeightTo = gs.WeightTo,
                    GovernorateName = gs.Governorate.Name
                }).ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}
