namespace BiyLineApi.Features.ReturnReceipt.Queries.GetReturnReceipts
{
    public class GetReturnReceiptsQuery
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
            public int ReturnReceiptId { get; set; }
            public int Number { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public double TotalCost { get; set; }
            public bool Recieved { get; set; }
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


                var query = _context.ReturnReceipts
                    .Include(r => r.ReturnReceiptDetails)
                    .Where(v => v.StoreId == store.Id)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(w => w.Id.ToString().Contains(request.Predicate));
                }

                return await PagedList<Response>.CreateAsync(
                     query.Select(v => new Response
                     {
                        Number = v.Number,
                        PaymentStatus = v.PaymentStatus,
                        ReturnReceiptId = v.Id,
                        TotalCost = v.TotalCostAfterDiscount??0.0,
                        VendorName = v.Vendor.Name
                     }).AsNoTracking(),
                    request.PageNumber,
                    request.PageSize);
            }
        }
    }
}
