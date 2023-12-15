namespace BiyLineApi.Features.TraderShippingCompany;
public sealed class DeleteShippingGovernorateFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ShippingGovernorateId { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.ShippingGovernorateId)
                .GreaterThan(0);
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

            var shippingGovernorate = await _context.GovernorateShippings
                .Include(c => c.TraderShippingCompany)
                .FirstOrDefaultAsync(c => c.Id == request.ShippingGovernorateId && c.TraderShippingCompany.StoreId == store.Id);

            if (shippingGovernorate == null)
            {
                return Result<Response>.Failure(new List<string> { "this governorate not exist for this store" });
            }

            _context.GovernorateShippings.Remove(shippingGovernorate);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
