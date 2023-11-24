namespace BiyLineApi.Features.Products;
public sealed class GetProductByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? GeneralOverview { get; set; }
        public string? Specifications { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public int? NumberOfReviews { get; set; }
        public DateTime? DateAdded { get; set; }
        public decimal? PriceBeforeDiscount { get; set; }
        public decimal? PriceAfterDiscount { get; set; }
        public decimal? Discount { get; set; }
        public string? SellerName { get; set; }
        public int? CountInStock { get; set; }
        public int? WarrantyMonths { get; set; }
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
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Store.Username))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Offer.DiscountPercentage))
                .ForMember(dest => dest.PriceBeforeDiscount, opt => opt.MapFrom(src => src.SellingPrice.Value))
                .ForMember(dest => dest.PriceAfterDiscount, opt =>
                    opt.MapFrom(src => src.SellingPrice.CalculatePriceAfterDiscount(src.Offer.DiscountPercentage.Value)))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.ProductTranslations.FirstOrDefault().Name))
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

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.ProductTranslations)
                .Include(p => p.Images)
                .Include(p => p.Store)
                .Where(p => p.Id == request.ProductId)
                .ProjectTo<Response>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (product is null)
            {
                // will be replaced with localization
                return Result<Response>.Failure("Product does not exist");
            }

            return Result<Response>.Success(product);
        }
    }
}
