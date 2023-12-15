
namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class GetTraderShippingCompaniesFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public sealed class Response
    {
        public string Name { get; set; }
    }
    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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
        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var traderId = _httpContextAccessor.GetUserById();
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);
            var query = _context.TraderShippingCompanies.Where(s=>s.StoreId == store.Id).AsQueryable();

            var traderShippingCompanies = query.Select(x => new Response
            {
                Name = x.Name,
            });

            return await PagedList<Response>.CreateAsync(
               traderShippingCompanies.AsNoTracking(),
               request.PageNumber.Value,
               request.PageSize.Value);
        }

    }
}
