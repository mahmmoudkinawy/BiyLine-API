namespace BiyLineApi.Features.Receipt.Queries.GetReceiptWarehouseLogs
{
    public class GetReturnReceiptWarehouseLogsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int ReceiptId { get; set; }
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

                var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == request.ReceiptId);
                //if (receipt == null)
                //{
                //    return Result<Response>.Failure(new List<string> { "receipt not found" });
                //}
                var warehouseLog = _context.WarehouseLogs.Include(w => w.Product).Where(w => w.WarehouseId == receipt.WarehouseId && w.DocumentId == receipt.Id && w.DocumentType == DocumentType.Receipt);


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
