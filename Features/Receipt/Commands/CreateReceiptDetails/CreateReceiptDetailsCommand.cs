namespace BiyLineApi.Features.Receipt.Commands.CreateReceiptDetails
{
    public class CreateReturnReceiptDetailsCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {

            public int ReceiptId { get; set; }
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
                        
                        var receipt = await _context.Receipts.FirstOrDefaultAsync(d => d.Id == request.ReceiptId, cancellationToken: cancellationToken);
                        if (receipt == null)
                        {
                            return Result<Response>.Failure(new List<string> { "Receipt Not Found" });
                        }
                        var detail = new ReceiptDetailsEntity();
                        detail.DiscountPercentage = request.DiscountPercentage;
                        detail.ProductId = request.ProductId;
                        detail.ProductVariationId = request.ProductVariationId;
                        detail.Quantity = request.Quantity;
                        detail.UnitCost = request.UnitCost;
                        detail.ReceiptId = request.ReceiptId;


                        var warehousecode = await _context.WarehouseLogs.FirstOrDefaultAsync(w => w.DocumentId == request.ReceiptId && w.DocumentType == DocumentType.Receipt);
                       
                        
                        var warehouseLog = new WarehouseLogEntity();

                        

                        warehouseLog.OperationDate = DateTime.UtcNow;
                        warehouseLog.ProductId = request.ProductId;
                        warehouseLog.ProductVariationId = request.ProductVariationId;
                        warehouseLog.Quantity = request.Quantity;
                        warehouseLog.Type = WarehouseLogType.In;
                        warehouseLog.DocumentType = DocumentType.Receipt;
                        warehouseLog.DocumentId = receipt.Id;
                        warehouseLog.WarehouseId = receipt.WarehouseId;
                        warehouseLog.SellingPrice = (decimal)request.UnitCost;
                        warehouseLog.Code = warehousecode == null ? Guid.NewGuid() : warehousecode.Code;
                        _context.ReceiptDetails.Add(detail);
                        _context.WarehouseLogs.Add(warehouseLog);

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
