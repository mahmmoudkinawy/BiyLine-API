namespace BiyLineApi.Features.Products;
public sealed class GetProductRatesByProductIdFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int ProductId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime? RatingDate { get; set; }

        // Hared coded for now!
        public string? OwnerName { get; set; } = "Bob Bob";
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<RateEntity, Response>();
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.Rates
                .OrderByDescending(r => r.RatingDate)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }

}
