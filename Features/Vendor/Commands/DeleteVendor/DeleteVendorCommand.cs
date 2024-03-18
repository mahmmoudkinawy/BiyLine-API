namespace BiyLineApi.Features.Vendor.Commands.DeleteVendor
{
    public class DeleteVendorCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
          
        }

        public sealed class Response
        {
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
                    var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Id == request.Id);
                    if (vendor == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Vendor Not Found" });
                    }
                  
                    _context.Vendors.Remove(vendor);
                    await _context.SaveChangesAsync();

                    return Result<Response>.Success(new Response {});
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }



            }
        }
    }
}
