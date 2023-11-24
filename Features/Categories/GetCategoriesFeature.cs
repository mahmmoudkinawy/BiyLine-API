namespace BiyLineApi.Features.Categories;
public sealed class GetCategoriesFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ImageResponse> Images { get; set; }
    }

    public sealed class ImageResponse
    {
        public string? FileName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageMimeType { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CategoryEntity, Response>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Take(2)));
            CreateMap<ImageEntity, ImageResponse>();
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
            var query = _context.Categories
                .OrderBy(c => c.Name)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
