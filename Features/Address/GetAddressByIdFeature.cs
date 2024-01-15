namespace BiyLineApi.Features.Address;

public sealed class GetAddressByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int Id { get; set; }
    }
    public sealed class Response
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetails { get; set; }
        public string Governorate { get; set; }
    }
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();



            var addressFromDb = await _context.Addresses.Include(a=>a.Governorate).FirstOrDefaultAsync(s => s.UserId == userId && s.Id == request.Id);
            if (addressFromDb == null)
            {
                return Result<Response>.Failure(new List<string> { "this address is not found" });
            }
            var address = new Response
            {
                FirstName = addressFromDb.FirstName,
                LastName = addressFromDb.LastName,
                PhoneNumber = addressFromDb.PhoneNumber,
                AddressDetails = addressFromDb.AddressDetails,
                Governorate = addressFromDb.Governorate.Name
        };

            return Result<Response>.Success(address);

        }
    }
}
