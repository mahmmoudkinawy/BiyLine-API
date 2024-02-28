namespace BiyLineApi.Features.Products.Queries;
public sealed class GetProductsByStoreIdFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int StoreId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public int? CountInStock { get; set; }
        public string? Brand { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public double? AverageRating { get; set; }
        public int? NumberOfReviews { get; set; }
        public DateTime? DateAdded { get; set; }
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
            CreateMap<ProductEntity, Response>()
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Description))
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Brand));
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
            var query = _context.Products
                .Include(p => p.ProductTranslations)
                .Where(p => p.StoreId == request.StoreId)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
