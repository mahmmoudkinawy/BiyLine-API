using BiyLineApi.Helpers;

namespace BiyLineApi.Features.ContractOrder;

public sealed class CancelContractOrderFeature
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

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure("This Store Is Not Found");
            }

            var contractOrderFromDb = await _context.ContractOrders.FirstOrDefaultAsync(c => c.Id == request.ContractOrderId && c.FromStoreId==traderId && c.Status == ContractOrderStatus.Pending.ToString());
            
            if(contractOrderFromDb == null)
            {
                return Result<Response>.Failure("This Contract Order Is Not Found");

            }
           
            _context.ContractOrders.Remove(contractOrderFromDb);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }
}
