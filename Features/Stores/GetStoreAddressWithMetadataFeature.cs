namespace BiyLineApi.Features.Stores;
public sealed class GetStoreAddressWithMetadataFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Governorate { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Activity { get; set; }
        public bool AcceptReturns { get; set; }
        public bool IsStoreAddressWithMetadataCompleted { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .Include(s => s.Governorate)
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store == null || store?.Address == null)
            {
                return new Response
                {
                    IsStoreAddressWithMetadataCompleted = false
                };
            }

            return new Response
            {
                Id = store.Id,
                AcceptReturns = store.AcceptsReturns.GetValueOrDefault(),
                Activity = store?.Activity.ToString(),
                Address = store?.Address,
                Governorate = store?.Governorate?.Name,
                Region = store?.Region.Name,
                IsStoreAddressWithMetadataCompleted = true
            };
        }
    }
}
