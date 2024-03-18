namespace BiyLineApi.Features.Vendor.Queries.GetVendors
{
    public class GetVendorsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public string? Predicate { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }

        }

        public sealed class Response
        {
            public string VendorName { get; set; }
            public int VendorId { get; set; }
            public double TotalReceiptAmount { get; set; }
            public double TotalReturnReceiptAmount { get; set; }
            public double NetAmount { get; set; }
        }

        public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly BiyLineDbContext _context;

            public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
            {
                _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                    var userId = _httpContextAccessor.GetUserById();
                    var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);
                  

                    var query = _context.Vendors
                        .Where(v => v.StoreId == store.Id)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(request.Predicate))
                    {
                        query = query.Where(w => w.Id.ToString().Contains(request.Predicate));
                    }

                    return await PagedList<Response>.CreateAsync(
                         query.Select(v => new Response
                         {
                            VendorId = v.Id,
                            VendorName = v.Name,
                            TotalReceiptAmount = v.ReceiptsCost,
                            TotalReturnReceiptAmount = v.ReturnReceiptsCost,
                            NetAmount = v.ReceiptsCost - v.ReturnReceiptsCost
                         }).AsNoTracking(),
                        request.PageNumber,
                        request.PageSize);
            }
        }
    }
}
