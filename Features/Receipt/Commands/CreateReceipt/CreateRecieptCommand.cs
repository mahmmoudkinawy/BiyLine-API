namespace BiyLineApi.Features.Receipt.Commands.CreateReceipt
{
    public class CreateReturnRecieptCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int StoreId { get; set; }
            public double? ValueAddedTax { get; set; }
            public double? TotalDiscountPercentage { get; set; }
            public int VendorId { get; set; }
            public int Number { get; set; }
            public DateTime CreatedDate { get; set; }
            public double ShippingCost { get; set; }
            public PaymentStatus PaymentStatus { get; set; }
            public double PaidAmount { get; set; }
            public int StoreWalletId { get; set; }
            public bool Recieved { get; set; }
            public DateTime RecievedDate { get; set; }
            public int WarehouseId { get; set; }
            public List<ReceiptDetailsVM> Details { get; set; } = new List<ReceiptDetailsVM>();
        }

        public sealed class Response
        {
            public ReceiptEntity Receipt { get; internal set; }
        }
        public class ReceiptDetailsVM
        {
            public int ReceiptId { get; set; }
            public int ProductId { get; set; }
            public int ProductVariationId { get; set; }
            public double UnitCost { get; set; }
            public double DiscountPercentage { get; set; }
            public double Quantity { get; set; }
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
                using(var trans = await _context.Database.BeginTransactionAsync())
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

                        var receipt = new ReceiptEntity
                        {
                            CreatedDate = DateTime.UtcNow,
                            Number = request.Number,
                            PaidAmount = request.PaidAmount,
                            PaymentStatus = request.PaymentStatus,
                            Recieved = request.Recieved,
                            RecievedDate = DateTime.UtcNow,
                            ShippingCost = request.ShippingCost,
                            StoreId = request.StoreId,
                            StoreWalletId = request.StoreWalletId,
                            TotalDiscountPercentage = request.TotalDiscountPercentage,
                            ValueAddedTax = request.ValueAddedTax,
                            VendorId = request.VendorId,
                            WarehouseId = request.WarehouseId,
                        };
                        _context.Receipts.Add(receipt);
                        await _context.SaveChangesAsync();
                        var Code = Guid.NewGuid();

                        foreach (var item in request.Details)
                        {
                            var detail = new ReceiptDetailsEntity
                            {
                                DiscountPercentage = item.DiscountPercentage,
                                ProductId = item.ProductId,
                                ProductVariationId = item.ProductVariationId,
                                Quantity = item.Quantity,
                                ReceiptId = receipt.Id,
                                UnitCost = item.UnitCost
                            };

                            var warehouseLog = new WarehouseLogEntity
                            {
                                Code = Code,
                                DocumentId = receipt.Id,
                                OperationDate = DateTime.UtcNow,
                                ProductId = item.ProductId,
                                ProductVariationId = item.ProductVariationId,
                                Quantity = item.Quantity,
                                Type = WarehouseLogType.In,
                                DocumentType = DocumentType.Receipt,
                                WarehouseId = request.WarehouseId,
                                SellingPrice = (decimal)item.UnitCost
                            };
                            
                            _context.ReceiptDetails.Add(detail);
                            _context.WarehouseLogs.Add(warehouseLog);

                        }
                        await _context.SaveChangesAsync();
                        await trans.CommitAsync();
                        return Result<Response>.Success(new Response { Receipt = receipt });
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
