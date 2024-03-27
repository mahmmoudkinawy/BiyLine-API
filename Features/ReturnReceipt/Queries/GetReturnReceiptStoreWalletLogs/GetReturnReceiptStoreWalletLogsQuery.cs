
namespace BiyLineApi.Features.ReturnReceipt.Queries.GetReturnReceiptStoreWalletLogs
{
    public class GetReturnReceiptStoreWalletLogsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int ReturnReceiptId { get; set; }
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

                var ReturnReceipt = await _context.ReturnReceipts.FirstOrDefaultAsync(r => r.Id == request.ReturnReceiptId);
                //if (ReturnReceipt == null)
                //{
                //    return Result<Response>.Failure(new List<string> { "ReturnReceipt not found" });
                //}
                var storeWalletLog = _context.StoreWalletLogs.Where(sw => sw.StoreWalletId == ReturnReceipt.StoreWalletId && sw.DocumentId == ReturnReceipt.Id && sw.DocumentType == DocumentType.ReturnReceipt).AsQueryable();


                return await PagedList<Response>.CreateAsync(
               storeWalletLog.Select(v => new Response
               {
                   No = ReturnReceipt.Number,
                   Paid = v.Value.GetValueOrDefault(),
                   Remaining = (decimal)ReturnReceipt.PaidAmount - v.Value.GetValueOrDefault()
               }).AsNoTracking(),
              request.PageNumber,
              request.PageSize);




            }
        }
    }
}
