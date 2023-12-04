using Microsoft.Identity.Client;

namespace BiyLineApi.Features.SupplierInvoice;

public sealed class UpdateSupplierInvoiceFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public decimal PaidAmount { get; set; }
        public decimal Returned { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.PaidAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(s => s.Returned)
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
            var traderId = _httpContextAccessor.GetUserById();

            var supplierInvoiceId = _httpContextAccessor.GetValueFromRoute("supplierInvoiceId");

            var supplierInvoice = await _context.SupplierInvoices.Include(i=>i.ContractOrder).FirstOrDefaultAsync(i => i.Id == supplierInvoiceId && i.ContractOrder.FromStoreId == traderId);

            if (supplierInvoice == null)
            {
                return Result<Response>.Failure("This SupplierInvoice Not Found");
            }

            if((supplierInvoice.PaidAmount + request.PaidAmount) > supplierInvoice.TotalPrice)
            {
                return Result<Response>.BadRequest("Invalid Operation");
            }
            supplierInvoice.PaidAmount += request.PaidAmount;
            supplierInvoice.Returned = request.Returned;

            supplierInvoice.TotalPrice -= supplierInvoice.Returned;

            supplierInvoice.RemainingAmount = supplierInvoice.TotalPrice - supplierInvoice.PaidAmount;

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }
}
