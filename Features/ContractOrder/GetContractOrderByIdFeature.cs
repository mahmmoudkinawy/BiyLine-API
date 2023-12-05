namespace BiyLineApi.Features.ContractOrder;

public sealed class GetContractOrderByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ContractOrderId { get; set; }
    }


    public sealed class Response
    {

        public string? Status { get; set; }

        public int FromStoreId { get; set; }

        public int ToStoreId { get; set; }

        public string? StoreName { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public decimal? TotalPrice { get; set; }

        public List<ImageResponse> Images { get; set; }
        public List<ProductResponse> Products { get; set; }

    }


    public sealed class ProductResponse
    {
        public int ProductId { get; set; }

        public decimal? TotalProductPrice { get; set; }

        public List<VariationResponse> Variations { get; set; }

    }

    public sealed class VariationResponse
    {
        public int Quantity { get; set; }

        public int ProductVariationId { get; set; }
    }

    public sealed class ImageResponse
    {
        public string? FileName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageMimeType { get; set; }
    }


    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                            throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                            throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var supplierId = _httpContextAccessor.GetUserById();



            var contractOrder = await _context.ContractOrders
                .Include(c => c.ToStore)
                .ThenInclude(s => s.Images)
                .Include(s => s.ContractOrderProducts)
                .ThenInclude(c => c.ContractOrderVariations)
                .FirstOrDefaultAsync(c => c.Id == request.ContractOrderId);

            if (contractOrder == null)
            {
                return Result<Response>.Failure("This Contract Order Not Found");

            }

            if (contractOrder.ToStoreId != supplierId)
            {
                return Result<Response>.Failure("This Supplier Does Not Have Permission");
            }

            var response = new Response
            {
                Status = contractOrder.Status,
                FromStoreId = contractOrder.FromStoreId,
                Note = contractOrder.Note,
                Date = contractOrder.Date,
                ToStoreId = contractOrder.ToStoreId,
                StoreName = contractOrder.ToStore.EnglishName,
                Images = contractOrder.ToStore.Images.Select(i => new ImageResponse
                {
                    FileName = i.FileName,
                    ImageUrl = i.ImageUrl,
                    ImageMimeType = i.ImageMimeType
                }).ToList(),

                TotalPrice = contractOrder.TotalPrice,
                Products = contractOrder.ContractOrderProducts.Select(cp => new ProductResponse
                {

                    ProductId = cp.ProductId,
                    TotalProductPrice = cp.ProductPrice,
                    Variations = cp.ContractOrderVariations.Select(v => new VariationResponse
                    {
                        ProductVariationId = v.ProductVariationId,
                        Quantity = v.Quantity
                    }).ToList()
                }).ToList(),
            };


            return Result<Response>.Success(response);


        }
    }
}


