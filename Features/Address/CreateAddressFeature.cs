
namespace BiyLineApi.Features.Address;

public sealed class CreateAddressFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetails { get; set; }
        public int GovernorateId { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {

        public Validator()
        {
            RuleFor(s => s.FirstName).NotEmpty();
            RuleFor(s => s.LastName).NotEmpty();
            RuleFor(s => s.PhoneNumber).NotEmpty();
            RuleFor(s => s.AddressDetails).NotEmpty();
            RuleFor(s => s.GovernorateId).GreaterThan(0);
        }
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

            var governorate = await _context.Governments.FirstOrDefaultAsync(g => g.Id == request.GovernorateId);
            if (governorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate is not exist" });
            }

            var addressToCreate = new AddressEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                AddressDetails = request.AddressDetails,
                GovernorateId = request.GovernorateId,
                UserId = userId,
            };

            _context.Addresses.Add(addressToCreate);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }

}
