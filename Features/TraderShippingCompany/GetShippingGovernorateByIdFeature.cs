namespace BiyLineApi.Features.TraderShippingCompany;
public sealed class GetShippingGovernorateByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ShippingGovernorateId { get; set; }
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
        public List<shippingCenterResponse> ShippingCenters { get; set; } = new List<shippingCenterResponse>();
    }
    public sealed class shippingCenterResponse
    {
        public int Id { get; set; }
        public string ShippingCenterName { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal PickupPrice { get; set; }
        public decimal ReturnCost { get; set; }
        public decimal WeightTo { get; set; }
        public decimal PricePerExtraKilo { get; set; }
        public string Status { get; set; }
    }
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
            var store = await _context.Stores
                .Include(s=>s.TraderShippingCompanies)
                .FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }

            var governorate =  await _context.GovernorateShippings
                .Include(g=>g.Governorate)
                .Include(g=>g.TraderShippingCompany)
                       .ThenInclude(t => t.Store)
                 .Include(g=>g.CenterShippings)
                .FirstOrDefaultAsync(s=>s.Id== request.ShippingGovernorateId && s.TraderShippingCompany.StoreId==store.Id);

            if(governorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate not exist" });
            }

            var response = new Response
            {
                Id = governorate.Id,
                GovernorateName = governorate.Governorate.Name,
                PickupPrice = governorate.PickupPrice,
                PricePerExtraKilo = governorate.PricePerExtraKilo,
                ReturnCost = governorate.ReturnCost,
                ShippingPrice = governorate.ShippingPrice,
                WeightTo = governorate.WeightTo,
                TraderShippingCompanyName = governorate.TraderShippingCompany.Name,
                ShippingCenters = governorate.CenterShippings.Select(c => new shippingCenterResponse
                {
                    Id = c.Id,
                    PickupPrice = c.PickupPrice,
                    PricePerExtraKilo = c.PricePerExtraKilo,
                    ReturnCost = c.ReturnCost,
                    ShippingPrice = c.ShippingPrice,
                    WeightTo = c.WeightTo,
                    Status = c.Status,
                    ShippingCenterName = c.Name
                }).ToList()
            };

            return Result<Response>.Success(response);
        }
    }
}
