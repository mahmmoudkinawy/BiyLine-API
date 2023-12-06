namespace BiyLineApi.Features.Contractor;

public sealed class GetAllContractorsFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int TraderId { get; set; }

        public string? TraderName { get; set; }

        public string? StoreName { get; set; }

        public string? PhoneNumber { get; set; }

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
            var supplierId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == supplierId);

            var query = _context.Stores
                .Include(s => s.Suppliers)
                .Include(s => s.Owner)
                .Where(s => s.Suppliers.Any(s => s.UserId == supplierId))
                .AsQueryable();

            var contractors = query.Select(s => new Response
            {
                TraderId = s.Id,
                TraderName = s.Owner.Name,
                StoreName = s.EnglishName,
                PhoneNumber = s.Owner.PhoneNumber,

            });


            return await PagedList<Response>.CreateAsync(
                contractors.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
