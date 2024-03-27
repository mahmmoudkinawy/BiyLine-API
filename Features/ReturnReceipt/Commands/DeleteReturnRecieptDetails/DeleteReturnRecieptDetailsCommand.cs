namespace BiyLineApi.Features.ReturnReceipt.Commands.DeleteReturnReturnReceiptDetails
{
    public class DeleteReturnReturnReceiptDetailsCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {

            public int Id { get; set; }
          
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
                using (var trans = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var userId = _httpContextAccessor.GetUserById();

                        var store = await _context.Stores
                            .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);
                        if (store == null)
                        {
                            await trans.RollbackAsync(cancellationToken);
                            return Result<Response>.Failure(new List<string> { "Store Not Found" });
                        }
                        var detail = await _context.ReturnReceiptDetails.Include(rd =>rd.ReturnReceipt).FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken: cancellationToken);
                        if (detail == null)
                        {
                            return Result<Response>.Failure(new List<string> { "ReturnReceipt Details Not Found" });
                        }


                        var warehouseLog = await _context.WarehouseLogs.FirstOrDefaultAsync(W => W.DocumentId == detail.ReturnReceiptId && W.DocumentType == DocumentType.ReturnReceipt && W.ProductId == detail.ProductId);

                        if (warehouseLog == null)
                        {
                            return Result<Response>.Failure(new List<string> { "Warehouse Log Not Found" });
                        }
                        _context.ReturnReceiptDetails.Remove(detail);
                        _context.WarehouseLogs.Remove(warehouseLog);

                        await _context.SaveChangesAsync();
                        await trans.CommitAsync();
                        return Result<Response>.Success(new Response { });
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync(cancellationToken);
                        return Result<Response>.Failure(new List<string> { ex.Message });
                    }

                }
            }
        }
    }
}
