namespace BiyLineApi.Features.ExpensesTypes;
public sealed class GetExpensesTypeFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public DateTime? Date { get; set; }
        public string? Type { get; set; }
        public string? Wallet { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Wallet { get; set; }
        public decimal Amount { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(
            IHttpContextAccessor httpContextAccessor,
            BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            var query = _context.ExpenseTypes
                .Where(e => e.StoreId == store.Id)
                .OrderBy(e => e.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Type))
            {
                query = query.Where(e => e.Type.Contains(request.Type));
            }

            if (!string.IsNullOrEmpty(request.Wallet))
            {
                query = query.Where(e => e.StoreWallet.Name.Contains(request.Wallet));
            }

            if (request.Date != null)
            {
                query = query.Where(e => e.Created.Month == request.Date.Value.Month);
            }

            var result = query.Select(e => new Response
            {
                Id = e.Id,
                Amount = e.Amount,
                Type = e.Type,
                Wallet = e.StoreWallet.Name
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
