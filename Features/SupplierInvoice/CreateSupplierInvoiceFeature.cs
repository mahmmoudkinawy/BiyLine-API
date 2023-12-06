namespace BiyLineApi.Features.SupplierInvoice;

public sealed class CreateSupplierInvoiceFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public decimal ShippingPrice { get; set; }
        public decimal PaidAmount { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.PaidAmount)
                .GreaterThan(0);

            RuleFor(s => s.ShippingPrice)
                .GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var contractOrderId = _httpContextAccessor.GetValueFromRoute("contractOrderId");

            var contractOrder = await _context.ContractOrders.FirstOrDefaultAsync(c => c.Id == contractOrderId);

            var supplierInvoice = new SupplierInvoiceEntity
            {
                ShippingPrice = request.ShippingPrice,
                PaidAmount = request.PaidAmount,

            };

            supplierInvoice.TotalPrice = supplierInvoice.ShippingPrice + contractOrder.TotalPrice;
            supplierInvoice.RemainingAmount = supplierInvoice.TotalPrice - supplierInvoice.PaidAmount;

            _context.SupplierInvoices.Add(supplierInvoice);
            await _context.SaveChangesAsync();

            contractOrder.SupplierInvoiceId = supplierInvoice.Id;
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
