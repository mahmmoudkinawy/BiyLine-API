namespace BiyLineApi.Features.Receipt.Commands.DeleteReciept
{
    public class DeleteReturnRecieptCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int ReceiptId { get; set; }
        }

        public sealed class Response
        {

        }


        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly BiyLineDbContext _context;

            public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
            {
                _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {

                try
                {
                    var userId = _httpContextAccessor.GetUserById();

                    var store = await _context.Stores
                        .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);
                    if (store == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Store Not Found" });
                    }

                    var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == request.ReceiptId);
                    if (receipt == null)
                    {
                        return Result<Response>.Failure(new List<string> {"receipt not found"});
                    }

                    var warehouseLogs = await _context.WarehouseLogs.Where(l => l.DocumentId == request.ReceiptId && l.DocumentType == DocumentType.Receipt).ToListAsync();
                    _context.Receipts.Remove(receipt);
                    _context.WarehouseLogs.RemoveRange(warehouseLogs);
                    await _context.SaveChangesAsync();

                    return Result<Response>.Success(new Response { });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }




            }
        }
    }
}
