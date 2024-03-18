namespace BiyLineApi.Features.Vendor.Queries.GetVendor
{
    public class GetVendorQuery
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }

        }

        public sealed class Response
        {
            public VendorEntity Vendor { get; set; }
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
                    var vendor = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(v => v.Id == request.Id);
                    if (vendor == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Vendor Not Found" });
                    }


                    return Result<Response>.Success(new Response { Vendor = vendor });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
