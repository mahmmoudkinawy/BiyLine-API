using Microsoft.Identity.Client;

namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class CreateTraderShippingCompanyFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string ShippingCompanyName { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s=>s.ShippingCompanyName)
                .NotEmpty();
        }
    }
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
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }

            if (_context.TraderShippingCompanies.Any(ts => ts.Name == request.ShippingCompanyName && ts.StoreId == store.Id))
            {
                return Result<Response>.BadRequest(new List<string> { "A shipping company with the same name already exists for this store" });
            }

            var traderShippingCompany = new TraderShippingCompanyEntity
            {
                Name = request.ShippingCompanyName,
                StoreId = store.Id
            };
            _context.TraderShippingCompanies.Add(traderShippingCompany);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
