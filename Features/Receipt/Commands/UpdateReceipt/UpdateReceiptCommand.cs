namespace BiyLineApi.Features.Receipt.Commands.UpdateReceipt
{
    public class UpdateReturnReceiptCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {

            public int Id { get; set; }
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
        }

        public sealed class Response
        {
            public ReceiptEntity Receipt { get; internal set; }
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

                        var receipt = await _context.Receipts.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);
                        if (receipt == null)
                        {
                            return Result<Response>.Failure(new List<string> { "Receipt Not Found" });
                        }

                        receipt.CreatedDate = DateTime.UtcNow;
                        receipt.Number = request.Number;
                        receipt.PaidAmount = request.PaidAmount;
                        receipt.PaymentStatus = request.PaymentStatus;
                        receipt.Recieved = request.Recieved;
                        receipt.RecievedDate = DateTime.UtcNow;
                        receipt.ShippingCost = request.ShippingCost;
                        receipt.StoreId = request.StoreId;
                        receipt.StoreWalletId = request.StoreWalletId;
                        receipt.TotalDiscountPercentage = request.TotalDiscountPercentage;
                        receipt.ValueAddedTax = request.ValueAddedTax;
                        receipt.VendorId = request.VendorId;
                        receipt.WarehouseId = request.WarehouseId;
                     
                        _context.Receipts.Update(receipt);
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
