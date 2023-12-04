namespace BiyLineApi.Features.Supplier;

public sealed class GetRetailSuppliersFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public sealed class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TradeName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SupplierType { get; set; }
        public bool IsSuspended { get; set; }
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
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            var query = _context.Suppliers
                .Where(s => s.StoreId == store.Id && s.SupplierType == SupplierTypeEnum.Inside.ToString())
                .Include(s => s.User)
                .ThenInclude(u => u.Store)
                .AsQueryable();

            var suppliers = query
                .Select(s => new Response
                {
                    Id = s.Id,
                    Name = s.Name ?? s.User.Name,
                    TradeName = s.TradeName ?? s.User.Store.EnglishName,
                    AccountNumber = s.AccountNumber,
                    PhoneNumber = s.PhoneNumber ?? s.User.PhoneNumber,
                    PaymentMethod = s.PaymentMethod,
                    SupplierType = s.SupplierType,
                    IsSuspended = s.IsSuspended
                })
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                suppliers.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
