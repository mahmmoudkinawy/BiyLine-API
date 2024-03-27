namespace BiyLineApi.Features.ReturnReceipt.Queries.GetReturnReceiptWarehouseLogs
{
    public class GetReturnReceiptWarehouseLogsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int ReturnReceiptId { get; set; }
        }


        public sealed class Response
        {
            public string ProductName { get; set; }
            public double  Quantity { get; set; }
            public decimal Cost { get; set; }
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
                var warehouseLog = _context.WarehouseLogs.Include(w => w.Product).Where(w =>  w.DocumentId == ReturnReceipt.Id && w.DocumentType == DocumentType.ReturnReceipt);


                return await PagedList<Response>.CreateAsync(
               warehouseLog.Select(v => new Response
               {
                  Cost = v.SellingPrice.GetValueOrDefault(),
                  ProductName = v.Product.CodeNumber,
                  Quantity = v.Quantity
               }).AsNoTracking(),
              request.PageNumber,
              request.PageSize);
            }
        }
    }
}
