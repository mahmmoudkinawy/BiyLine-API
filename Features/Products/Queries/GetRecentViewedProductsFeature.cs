namespace BiyLineApi.Features.Products.Queries;
public sealed class GetRecentViewedProductsFeature
{
    public sealed class Request : IRequest<IEnumerable<Response>> { }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public decimal? PriceBeforeDiscount { get; set; }
        public decimal? PriceAfterDiscount { get; set; }
        public decimal? Discount { get; set; }
        public int? CountInStock { get; set; }
        public string? Brand { get; set; }
        public string? SellerName { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ProductEntity, Response>()
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Store.Username))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Offer.DiscountPercentage))
                .ForMember(dest => dest.PriceBeforeDiscount, opt => opt.MapFrom(src => src.SellingPrice.Value))
                .ForMember(dest => dest.PriceAfterDiscount, opt =>
                    opt.MapFrom(src => src.SellingPrice.CalculatePriceAfterDiscount(src.Offer.DiscountPercentage.Value)))
                .ForMember(dest => dest.ImageUrl, opt =>
                    opt.MapFrom(src => src.Images.OrderByDescending(i => i.DateUploaded).FirstOrDefault(i => i.IsMain.Value).ImageUrl))
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Brand));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IEnumerable<Response>>
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

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var recentViews = _httpContextAccessor.HttpContext.Request.Cookies["recentViews"];
            var productIds = string.IsNullOrEmpty(recentViews) ?
                new List<int>()
                :
                recentViews.Split(',').Select(int.Parse).Take(15).ToList();

            if (!productIds.Any())
            {
                return Enumerable.Empty<Response>();
            }

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ProjectTo<Response>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken);

            return products;
        }
    }

}
