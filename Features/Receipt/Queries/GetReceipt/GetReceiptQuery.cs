
namespace BiyLineApi.Features.Receipt.Queries.GetReceipt
{
    public class GetReturnReceiptQuery
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int ReceiptId { get; set; }
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

                try
                {
                    var userId = _httpContextAccessor.GetUserById();

                    var store = await _context.Stores
                        .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);
                    if (store == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Store Not Found" });
                    }

                    var receipt = await _context.Receipts
                        .Include(r => r.ReceiptDetails)
                        .Include(r => r.Vendor)
                        
                        .FirstOrDefaultAsync(r => r.Id == request.ReceiptId);
                    if (receipt == null)
                    {
                        return Result<Response>.Failure(new List<string> { "receipt not found" });
                    }
                  

                    return Result<Response>.Success(new Response { Receipt = receipt });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }




            }
        }
    }
}
