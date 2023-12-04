namespace BiyLineApi.Features.Trader;

public sealed class UpdateContractOrderStatusToDeliverdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ContractOrderId { get; set; }
    }

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
            var traderId = _httpContextAccessor.GetUserById();

            var contractOrder = await _context.ContractOrders.FirstOrDefaultAsync(c => c.Id == request.ContractOrderId && c.FromStoreId == traderId);

            if (contractOrder == null)
            {
                return Result<Response>.Failure("This Contract Order Not Found");
            }

            contractOrder.Status = ContractOrderStatus.Delivered.ToString();

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
