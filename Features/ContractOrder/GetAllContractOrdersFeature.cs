
using AutoMapper.Configuration.Conventions;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BiyLineApi.Features.ContractOrder;

public sealed class GetAllContractOrdersFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }


    public sealed class Response
    {
        public int ContractOrderId { get; set; }

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


    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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
        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var supplierId = _httpContextAccessor.GetUserById();

            var query = _context.ContractOrders
                .Include(c => c.ToStore)
                .ThenInclude(s => s.Images)
                .Include(s => s.ContractOrderProducts)
                .Where(c => c.ToStoreId == supplierId)
                .AsQueryable();


            var contractOrders = query.Select(c => new Response
            {
                ContractOrderId = c.Id,
                Status = c.Status,
                FromStoreId = c.FromStoreId,
                Note = c.Note,
                Date = c.Date,
                ToStoreId = c.ToStoreId,
                StoreName = c.ToStore.EnglishName,
                Images = c.ToStore.Images.Select(i => new ImageResponse
                {
                    FileName = i.FileName,
                    ImageUrl = i.ImageUrl,
                    ImageMimeType = i.ImageMimeType
                }).ToList(),

                TotalPrice = c.TotalPrice,
                Products = c.ContractOrderProducts.Select(cp => new ProductResponse
                {

                    ProductId = cp.ProductId,
                    TotalProductPrice = cp.ProductPrice,
                    Variations = cp.ContractOrderVariations.Select(v => new VariationResponse
                    {
                        ProductVariationId = v.ProductVariationId,
                        Quantity = v.Quantity
                    }).ToList()
                }).ToList(),
            });



            return await PagedList<Response>.CreateAsync(
               contractOrders.AsNoTracking(),
               request.PageNumber.Value,
               request.PageSize.Value);


        }
    }
}
