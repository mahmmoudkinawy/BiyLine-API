
namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class GetAllGovernoratesForTraderShippingCompanyFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>,Result<Response>>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public sealed class Response
    {
        public int Id { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
        public string GovernorateName { get; set; }
        public string TraderShippingCompanyName { get; set; }
    }
    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
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

        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var traderId = _httpContextAccessor.GetUserById();
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }
            var traderShippingCompanyId = _httpContextAccessor.GetValueFromRoute("traderShippingCompanyId");
            var traderShippingCompany = await _context.TraderShippingCompanies.FirstOrDefaultAsync(s => s.Id == traderShippingCompanyId && s.StoreId==store.Id);

            if (traderShippingCompany == null)
            {
                return Result<Response>.Failure(new List<string> { "this shipping company not found" });
            }

            var query = _context.GovernorateShippings
                .Where(s=>s.TraderShippingCompanyId==traderShippingCompanyId)
                .AsQueryable();

            var governorates = query.Select(s => new Response
            {
                Id = s.Id,
                TraderShippingCompanyName = s.TraderShippingCompany.Name,
                GovernorateName = s.Governorate.Name,
                PickupPrice = s.PickupPrice,
                PricePerExtraKilo = s.PricePerExtraKilo,
                ShippingPrice = s.ShippingPrice,
                WeightTo = s.WeightTo,
                ReturnCost = s.ReturnCost
            });

            return await PagedList<Response>.CreateAsync(
                governorates.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
