namespace BiyLineApi.Features.Address;

public sealed class DeleteAddressFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int AddressId { get; set; }
    }
    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s=>s.AddressId).GreaterThan(0);
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

            var address = await _context.Addresses.FirstOrDefaultAsync(s => s.Id == request.AddressId && s.UserId==userId);

            if (address == null)
            {
                return Result<Response>.Failure(new List<string> { "this address is not found" });
            }

            _context.Remove(address);

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }


}
