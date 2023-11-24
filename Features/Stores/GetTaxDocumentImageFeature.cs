namespace BiyLineApi.Features.Stores;
public sealed class GetTaxDocumentImageFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public string ImageUrl { get; set; }
        public bool IsTaxDocumentImageCompleted { get; set; }
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
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var storeTaxDocumentImage = await _context.Images
                .FirstOrDefaultAsync(i =>
                    i.IsMain.Value && i.StoreId == store.Id && i.Type == "TaxRegistrationDocumentImage",
                        cancellationToken: cancellationToken);

            if (store == null || storeTaxDocumentImage == null)
            {
                return new Response
                {
                    IsTaxDocumentImageCompleted = false
                };
            }

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            return new Response
            {
                ImageUrl = baseUri.CombineUri(storeTaxDocumentImage?.ImageUrl),
                IsTaxDocumentImageCompleted = true
            };
        }
    }
}
