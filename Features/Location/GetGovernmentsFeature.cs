namespace BiyLineApi.Features.Location;
public sealed class GetGovernoratesFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int CountryId { get; set; }
        public string? Predicate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GovernorateCode { get; set; }
        public int CountryId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<GovernorateEntity, Response>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;

        public Handler(
            BiyLineDbContext context,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var query = _context.Governments
                .Where(c => c.CountryId == request.CountryId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(c => c.Name.Equals(request.Predicate));
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
