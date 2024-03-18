
namespace BiyLineApi.Features.Vendor.Commands.CreateVendor
{
    public class CreateVendorCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
        }

        public sealed class Response
        {
            public VendorEntity Vendor { get; internal set; }
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

                    var vendor = new VendorEntity
                    {
                        StoreId = store.Id,
                        Name = request.Name,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        Address = request.Address
                    };
                    _context.Vendors.Add(vendor);
                    await _context.SaveChangesAsync();

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
