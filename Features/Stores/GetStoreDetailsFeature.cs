namespace BiyLineApi.Features.Stores;
public sealed class GetStoreDetailsFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string ArabicStoreName { get; set; }
        public string EnglishStoreName { get; set; }
        public string StoreUsername { get; set; }
        public List<CategoryResponse> Categories { get; set; }
        public bool IsStoreDetailsCompleted { get; set; }
    }

    public sealed class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
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
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return new Response
                {
                    IsStoreDetailsCompleted = false
                };
            }

            var storeCategories = await _context.StoreCategories
                .Include(sc => sc.Category)
                .Where(sc => sc.StoreId == store.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            return new Response
            {
                Id = store.Id,
                CountryId = store.CountryId.Value,
                ArabicStoreName = store?.ArabicName,
                Country = store?.Country?.Name,
                EnglishStoreName = store?.EnglishName,
                StoreUsername = store?.Username,
                Categories = storeCategories.Select(sc => new CategoryResponse
                {
                    Id = sc.CategoryId,
                    Name = sc.Category.Name
                }).ToList(),
                IsStoreDetailsCompleted = true
            };
        }
    }

}
