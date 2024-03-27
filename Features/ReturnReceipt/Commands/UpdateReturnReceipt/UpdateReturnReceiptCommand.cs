namespace BiyLineApi.Features.ReturnReceipt.Commands.UpdateReturnReceipt
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
            public ReturnReceiptEntity ReturnReceipt { get; internal set; }
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

                        var ReturnReceipt = await _context.ReturnReceipts.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);
                        if (ReturnReceipt == null)
                        {
                            return Result<Response>.Failure(new List<string> { "ReturnReceipt Not Found" });
                        }

                        ReturnReceipt.CreatedDate = DateTime.UtcNow;
                        ReturnReceipt.Number = request.Number;
                        ReturnReceipt.PaidAmount = request.PaidAmount;
                        ReturnReceipt.PaymentStatus = request.PaymentStatus;
 
                        ReturnReceipt.ShippingCost = request.ShippingCost;
                        ReturnReceipt.StoreId = request.StoreId;
                        ReturnReceipt.StoreWalletId = request.StoreWalletId;
                        ReturnReceipt.TotalDiscountPercentage = request.TotalDiscountPercentage;
                        ReturnReceipt.ValueAddedTax = request.ValueAddedTax;
                        ReturnReceipt.VendorId = request.VendorId;
                     
                        _context.ReturnReceipts.Update(ReturnReceipt);
                        await _context.SaveChangesAsync();
                        await trans.CommitAsync();
                        return Result<Response>.Success(new Response { ReturnReceipt = ReturnReceipt });
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
