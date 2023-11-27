namespace BiyLineApi.Features.Stores;
public sealed class GetStoreSpecializationFeature
{
    public sealed class Request : IRequest<IEnumerable<Response>> { }
    public sealed class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<SubSpecializationResponse>? SubSpecializations { get; set; }
    }

    public sealed class SubSpecializationResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var specializations = await _context.Specializations
                .Include(s => s.SubSpecializations)
                    .ThenInclude(ss => ss.SubSpecializationImage)
                .Where(s => s.StoreId == store.Id)
                .OrderBy(s => s.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            if (store == null || !specializations.Any())
            {
                return Enumerable.Empty<Response>();
            }

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            var result = specializations.Select(specialization => new Response
            {
                Id = specialization.Id,
                Name = specialization?.Name,
                SubSpecializations = specialization?.SubSpecializations.Select(ss => new SubSpecializationResponse
                {
                    Id = ss.Id,
                    Name = ss.Name,
                    ImageUrl = baseUri.CombineUri(ss.SubSpecializationImage?.ImageUrl)
                }).ToList(),
            }).ToList();

            return result;
        }
    }
}
