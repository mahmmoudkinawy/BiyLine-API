

namespace BiyLineApi.Features.Address;

public class GetAddressesForCustomerFeature
{
    public sealed class Request : IRequest< PagedList<Response> >
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetails { get; set; }
        public string Governorate { get; set; }
        public int GovernorateId { get; set; }


    }
    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var query = _context.Addresses.Where(s => s.UserId == userId);

            var  addresses = query.Select(s => new Response
            {
                Id = s.Id,
                Name = s.User.Name,
                PhoneNumber = s.PhoneNumber,
                AddressDetails = s.AddressDetails,
                Governorate = s.Governorate.Name,
                GovernorateId = s.GovernorateId
            });

            return await PagedList<Response>.CreateAsync(addresses, request.PageNumber.Value, request.PageSize.Value);

        }
    }
}
