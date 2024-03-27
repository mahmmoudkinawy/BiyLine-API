namespace BiyLineApi.Features.ReturnReceipt.Commands.DeleteReturnReceipt
{
    public class DeleteReturnReturnReturnReceiptCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int ReturnReceiptId { get; set; }
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

                    var ReturnReceipt = await _context.ReturnReceipts.FirstOrDefaultAsync(r => r.Id == request.ReturnReceiptId);
                    if (ReturnReceipt == null)
                    {
                        return Result<Response>.Failure(new List<string> {"ReturnReceipt not found"});
                    }

                    var warehouseLogs = await _context.WarehouseLogs.Where(l => l.DocumentId == request.ReturnReceiptId && l.DocumentType == DocumentType.ReturnReceipt).ToListAsync();
                    _context.ReturnReceipts.Remove(ReturnReceipt);
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
