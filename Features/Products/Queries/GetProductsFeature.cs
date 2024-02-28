namespace BiyLineApi.Features.Products.Queries;
public sealed class GetProductsFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public bool? IsInStock { get; set; }
        public string? Predicate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? GeneralOverview { get; set; }
        public string? Specifications { get; set; }
        public int? CountInStock { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public int? NumberOfReviews { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? CategoryId { get; set; }
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
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Brand))
                .ForMember(dest => dest.GeneralOverview,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().GeneralOverview))
                .ForMember(dest => dest.Specifications,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Specifications));
            CreateMap<ImageEntity, ImageResponse>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMapper _mapper;

        public Handler(
            BiyLineDbContext context,
            IDateTimeProvider dateTimeProvider,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ??
                throw new ArgumentNullException(nameof(dateTimeProvider));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request,CancellationToken cancellationToken)
        {
            var query = _context.Products
                .Include(c => c.ProductTranslations)
                .AsQueryable();

            var currentDateTime = _dateTimeProvider.GetCurrentDateTimeUtc();

            query = request.Predicate switch
            {
                "hot" => query
                    .Where(p => p.Offer != null && p.Offer.StartDate <= currentDateTime && currentDateTime <= p.Offer.EndDate)
                    .OrderByDescending(p => p.DateAdded),
                _ => query
            };

            if (request.IsInStock.Value)
            {
                query = query.Where(p => p.CountInStock > 0);
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
