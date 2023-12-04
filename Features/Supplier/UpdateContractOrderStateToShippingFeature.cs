namespace BiyLineApi.Features.Supplier;

public sealed class UpdateContractOrderStateToShippingFeature
{
    public sealed class Request : IRequest<Result<Response>> { }

    public sealed class Response { }

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
            var supplierId = _httpContextAccessor.GetUserById();

            var contractOrderId = _httpContextAccessor.GetValueFromRoute("contractOrderId");

            var contractOrder = await _context.ContractOrders.FirstOrDefaultAsync(c => c.Id == contractOrderId && c.ToStoreId==supplierId);

            if (contractOrder == null) 
            {
                return Result<Response>.Failure("This Contract Order Not Found");
            }

            contractOrder.Status = ContractOrderStatus.Shipping.ToString();

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }

}
