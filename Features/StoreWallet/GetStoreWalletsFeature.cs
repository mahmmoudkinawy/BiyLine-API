namespace BiyLineApi.Features.StoreWallet;
public sealed class GetStoreWalletsFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public string? Predicate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string StoreWalletName { get; set; }
        public decimal? TotalBalance { get; set; }
        public DateTime DateTime { get; set; }
        public string StoreWalletStatus { get; set; }
        public string EmployeeName { get; set; }
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
            int traderId = _httpContextAccessor.GetUserById();

            var trader = await _context.Users
                .Include(u => u.Store)
                .FirstOrDefaultAsync(s => s.Id == traderId && s.StoreId != null, cancellationToken: cancellationToken);

            if (trader == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader is not found" });
            }

            var query = _context.StoreWallets
                .Where(s => s.StoreId == trader.StoreId)
                .OrderBy(s => s.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(s => s.Name.Contains(request.Predicate));
            }

            var storeWallets = query.Select(s => new Response
            {
                Id = s.Id,
                StoreWalletName = s.Name,
                TotalBalance = s.TotalBalance,
                DateTime = s.DateTime,
                StoreWalletStatus = s.StoreWalletStatus,
                EmployeeName = s.Employee.User.Name
            });

            return await PagedList<Response>.CreateAsync(
                storeWallets.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}

