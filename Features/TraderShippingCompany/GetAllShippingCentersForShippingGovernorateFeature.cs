
namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class GetAllShippingCentersForShippingGovernorateFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
        public string Status { get; set; }
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

            var shippingGovernorateId = _httpContextAccessor.GetValueFromRoute("shippingGovernorateId");
            var shippingGovernorate = await _context.GovernorateShippings.
                FirstOrDefaultAsync(g => g.Id == shippingGovernorateId && g.TraderShippingCompany.StoreId == store.Id);

            if (shippingGovernorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate shipping not exist for this store" });
            }

            var query = _context.CenterShippings
                .Where(g => g.GovernorateShippingId == shippingGovernorateId)
                .AsQueryable();

            var shippingCenters = query.Select(s => new Response
            {
                Id = s.Id,
                Name = s.Name,
                PickupPrice = s.PickupPrice,
                PricePerExtraKilo = s.PricePerExtraKilo,
                ReturnCost = s.ReturnCost,
                ShippingPrice = s.ShippingPrice,
                WeightTo = s.WeightTo,
                Status = s.Status
            });

            return await PagedList<Response>.CreateAsync(
                shippingCenters.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
