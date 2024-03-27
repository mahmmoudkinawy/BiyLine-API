using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BiyLineApi.Features.Receipt.Queries.GetReceiptStoreWalletLogs
{
    public class GetReturnReceiptStoreWalletLogsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int ReceiptId { get; set; }
        }


        public sealed class Response
        {
            public int No { get; set; }
            public decimal Paid { get; set; }
            public decimal Remaining { get; set; }
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

                var store = await _context.Stores
                    .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);
                //if (store == null)
                //{
                //    return Result<Response>.Failure(new List<string> { "Store Not Found" });
                //}

                var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == request.ReceiptId);
                //if (receipt == null)
                //{
                //    return Result<Response>.Failure(new List<string> { "receipt not found" });
                //}
                var storeWalletLog = _context.StoreWalletLogs.Where(sw => sw.StoreWalletId == receipt.StoreWalletId && sw.DocumentId == receipt.Id && sw.DocumentType == DocumentType.Receipt).AsQueryable();


                return await PagedList<Response>.CreateAsync(
               storeWalletLog.Select(v => new Response
               {
                   No = receipt.Number,
                   Paid = v.Value.GetValueOrDefault(),
                   Remaining = (decimal)receipt.PaidAmount - v.Value.GetValueOrDefault()
               }).AsNoTracking(),
              request.PageNumber,
              request.PageSize);




            }
        }
    }
}
