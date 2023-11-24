namespace BiyLineApi.Features.Products;
public sealed class GetProductDetailsByProductIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string CodeNumber { get; set; }
        public string Supplier { get; set; } = "Supplier fake user";
        public int ThresholdReached { get; set; }
        public string Category { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public string ImageUrl { get; set; }
        public List<ColorResponse> Colors { get; set; }
        public List<SizeResponse> Sizes { get; set; }
    }

    public sealed class ColorResponse
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
    }

    public sealed class SizeResponse
    {
        public int Id { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ProductEntity, Response>()
                .ForMember(dest => dest.ImageUrl, opts => opts.MapFrom(src =>
                    src.Images.OrderByDescending(i => i.DateUploaded).FirstOrDefault(i => i.Type == "ProductImage" && i.IsMain.Value)));
            CreateMap<ProductColorEntity, ColorResponse>();
            CreateMap<ProductSizeEntity, SizeResponse>();
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == request.UserId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "There is not store for the current user." });
            }

            var productBelongsToCurrentUserStore = await _context.Products
                .Include(c => c.Category)
                .FirstOrDefaultAsync(product =>
                    product.StoreId == store.Id && product.Id == request.ProductId, cancellationToken: cancellationToken);

            if (productBelongsToCurrentUserStore is null)
            {
                return Result<Response>.Failure(new List<string> { "This product does not belong to current logged in user." });
            }

            productBelongsToCurrentUserStore.Images = await _context
                .Images
                .OrderByDescending(i => i.DateUploaded)
                .Where(i => i.Type == "ProductImage" && i.ProductId == request.ProductId)
                .ToListAsync(cancellationToken: cancellationToken);

            productBelongsToCurrentUserStore.Colors = await _context
                .ProductColors
                .Where(pc => pc.ProductId == request.ProductId)
                .ToListAsync(cancellationToken: cancellationToken);

            productBelongsToCurrentUserStore.Sizes = await _context
                .ProductSizes
                .Where(ps => ps.ProductId == request.ProductId)
                .ToListAsync(cancellationToken: cancellationToken);

            productBelongsToCurrentUserStore.ProductTranslations = await _context
                .ProductTranslations
                .Where(pt => pt.ProductId == request.ProductId)
                .ToListAsync(cancellationToken: cancellationToken);

            var result = _mapper.Map<Response>(productBelongsToCurrentUserStore);
            result.Name = productBelongsToCurrentUserStore.ProductTranslations.FirstOrDefault().Name;
            result.Category = productBelongsToCurrentUserStore.Category?.Name;

            return Result<Response>.Success(result);
        }
    }
}
