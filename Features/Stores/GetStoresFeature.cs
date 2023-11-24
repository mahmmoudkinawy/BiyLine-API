namespace BiyLineApi.Features.Stores;
public sealed class GetStoresFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public decimal Rates { get; set; }
        public int ExperienceInYears { get; set; }
        public int NumberOfEmployees { get; set; }
        public int MinimumNumberOfPieces { get; set; }
        public double Rating { get; set; }
        public string ImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<StoreEntity, Response>()
                .ForMember(dest => dest.ImageUrl, 
                    opt => 
                        opt.MapFrom(src => src.Images.OrderByDescending(i => i.DateUploaded).FirstOrDefault().ImageUrl));
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
            var query = _context.Stores
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
