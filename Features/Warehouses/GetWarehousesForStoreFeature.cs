namespace BiyLineApi.Features.Warehouses;
public sealed class GetWarehousesForStoreFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public string? Predicate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShippingAddress { get; set; }
        public string WarehouseStatus { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<WarehouseEntity, Response>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var query = _context.Warehouses
                .Where(w => w.StoreId == store.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(w => w.Name.Contains(request.Predicate));
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
