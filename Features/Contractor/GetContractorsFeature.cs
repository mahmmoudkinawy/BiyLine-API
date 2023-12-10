namespace BiyLineApi.Features.Contractor;
public sealed class GetContractorsFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int TraderId { get; set; }
        public string? TraderName { get; set; }
        public string? StoreName { get; set; }
        public string? PhoneNumber { get; set; }
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
        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var supplierId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == supplierId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            var query = _context.Stores
                .Include(s => s.Suppliers)
                .Include(s => s.Owner)
                .Where(s => s.Id == store.Id && s.Suppliers.Any(s => s.UserId == supplierId))
                .AsQueryable();

            if (!await query.AnyAsync(cancellationToken: cancellationToken))
            {
                return Result<Response>.Success(new Response { });
            }

            var contractors = query.Select(s => new Response
            {
                TraderId = s.Id,
                TraderName = s.Owner.Name,
                StoreName = s.EnglishName,
                PhoneNumber = s.Owner.PhoneNumber
            });

            return await PagedList<Response>.CreateAsync(
                contractors.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
