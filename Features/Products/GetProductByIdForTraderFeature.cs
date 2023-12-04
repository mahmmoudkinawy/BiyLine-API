using System.Diagnostics.Contracts;

namespace BiyLineApi.Features.Products;

public sealed class GetProductByIdForTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public int? CountInStock { get; set; }
        public bool? IsInStock { get; set; }
        public int? NumberOfReviews { get; set; }
        public int? WarrantyMonths { get; set; }
        public string? CodeNumber { get; set; }
        public decimal? Vat { get; set; }
        public int? ThresholdReached { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? CategoryId { get; set; }
        public int? OfferId { get; set; }
        public int? SubcategoryId { get; set; }
        public int? WarehouseId { get; set; }
        public ICollection<ImageResponse> Images { get; set; }
        public ICollection<ProductVariationResponse> ProductVariations { get; set; } = new List<ProductVariationResponse>();
        public ICollection<QuantityPricingTierResponse> QuantityPricingTiers { get; set; } = new List<QuantityPricingTierResponse>();
        public ICollection<ProductTranslationResponse> ProductTranslations { get; set; } = new List<ProductTranslationResponse>();
        public ICollection<ContractOrderProductResponse> ContractOrderProducts { get; set; } = new List<ContractOrderProductResponse>();

    }

    public sealed class ImageResponse
    {
        public string? FileName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageMimeType { get; set; }
    }

    public sealed class RateResponse
    {
        public int Id { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime? RatingDate { get; set; }
        public int? ProductId { get; set; }
    }

    public sealed class ProductVariationResponse
    {
        public int Id { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int? Quantity { get; set; }
        public int ProductId { get; set; }
    }

    public sealed class QuantityPricingTierResponse
    {
        public int Id { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public decimal? Price { get; set; }

        public int ProductId { get; set; }
    }

    public sealed class ProductTranslationResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? GeneralOverview { get; set; }
        public string? Specifications { get; set; }
        public string Language { get; set; }
        public int ProductId { get; set; }

    }

    public sealed class ContractOrderProductResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;

        public Handler(
            BiyLineDbContext context
            )
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.ProductTranslations)
                .Include(p => p.Images)
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Subcategory)
                .Include(p => p.ProductVariations)
                .Include(p => p.Offer)
                .Include(p => p.ContractOrderProducts)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId);

            if (product is null)
            {
                return Result<Response>.Failure("Product does not exist");
            }

            var response = new Response
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CodeNumber = product.CodeNumber,
                CountInStock = product.CountInStock,
                DateAdded = product.DateAdded,
                Weight = product.Weight,
                Dimensions = product.Dimensions,
                IsInStock = product.IsInStock,
                OfferId = product.OfferId,
                ContractOrderProducts = product.ContractOrderProducts
                .Select(p => new ContractOrderProductResponse { Id = p.Id, ProductId = p.ProductId })
                .ToList(),

                Images = product.Images
                 .Select(p => new ImageResponse { FileName = p.FileName, ImageMimeType = p.ImageMimeType, ImageUrl = p.ImageUrl })
                .ToList(),

                NumberOfReviews = product.NumberOfReviews,
                OriginalPrice = product.OriginalPrice,
                ProductTranslations = product.ProductTranslations
                .Select(p => new ProductTranslationResponse { Id = p.Id,
                    Brand = p.Brand,
                    Description = p.Description,
                    GeneralOverview = p.GeneralOverview,
                    Language = p.Language,
                    Name = p.Name,
                    ProductId = p.ProductId,
                    Specifications = p.Specifications }).ToList(),

                ProductVariations = product.ProductVariations.Select(p => new ProductVariationResponse
                {
                    Id = p.Id,
                    ProductId = p.ProductId,
                    Color = p.Color,
                    Quantity = p.Quantity,
                    Size = p.Size
                }).ToList(),

                QuantityPricingTiers = product.QuantityPricingTiers.Select(p=>new QuantityPricingTierResponse
                {
                    Id=p.Id,
                    Price = p.Price,
                    MaxQuantity=p.MaxQuantity,
                    MinQuantity=p.MinQuantity,
                    ProductId = p.ProductId
                }).ToList(),

                SellingPrice = product.SellingPrice,
                SubcategoryId = product.SubcategoryId,
                ThresholdReached = product.ThresholdReached,
                Vat = product.Vat,
                WarehouseId = product.WarehouseId,
                WarrantyMonths = product.WarrantyMonths
            };

            return Result<Response>.Success(response);
        }
    }

}

