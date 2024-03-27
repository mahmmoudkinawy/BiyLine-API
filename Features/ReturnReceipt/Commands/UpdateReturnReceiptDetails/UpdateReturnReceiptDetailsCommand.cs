namespace BiyLineApi.Features.ReturnReceipt.Commands.UpdateReturnReceiptDetails
{
    public class UpdateReturnReceiptDetailsCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {

            public int Id { get; set; }
            public int ProductId { get; set; }
            public int ProductVariationId { get; set; }
            public double UnitCost { get; set; }
            public double DiscountPercentage { get; set; }
            public double Quantity { get; set; }
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
                        var detail = await _context.ReturnReceiptDetails.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken: cancellationToken);
                        if (detail == null)
                        {
                            return Result<Response>.Failure(new List<string> { "ReturnReceipt Details Not Found" });
                        }

                        detail.DiscountPercentage = request.DiscountPercentage;
                        detail.ProductId = request.ProductId;
                        detail.ProductVariationId = request.ProductVariationId;
                        detail.Quantity = request.Quantity;
                        detail.UnitCost = request.UnitCost;


                        var warehouseLog = await _context.WarehouseLogs.FirstOrDefaultAsync(W => W.DocumentId == detail.ReturnReceiptId && W.DocumentType == DocumentType.ReturnReceipt);

                        if(warehouseLog == null)
                        {
                            return Result<Response>.Failure(new List<string> { "Warehouse Log Not Found" });
                        }

                        warehouseLog.OperationDate = DateTime.UtcNow;
                        warehouseLog.ProductId = request.ProductId;
                        warehouseLog.ProductVariationId = request.ProductVariationId;
                        warehouseLog.Quantity = request.Quantity;
                        warehouseLog.Type = WarehouseLogType.Out;

                        warehouseLog.SellingPrice = (decimal)request.UnitCost;
                
                        _context.ReturnReceiptDetails.Update(detail);
                        _context.WarehouseLogs.Update(warehouseLog);

                        await _context.SaveChangesAsync();
                        await trans.CommitAsync();
                        return Result<Response>.Success(new Response {  });
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
