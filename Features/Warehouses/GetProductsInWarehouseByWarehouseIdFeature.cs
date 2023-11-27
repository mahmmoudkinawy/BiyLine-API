namespace BiyLineApi.Features.Warehouses;
public sealed class GetWarehouseProductByWarehouseIdFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public required int WarehouseId { get; set; }
        public string? OrderBy { get; set; } = "priceAsc";
        public string? CodeNumber { get; set; }
        public string? Name { get; set; }
        public bool? IsInStockStatus { get; set; } = true;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? CodeNumber { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsInStockStatus { get; set; }
        public decimal? SellingPrice { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.WarehouseId)
                .GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen a store." });
            }

            var query = _context.Warehouses
                .IgnoreQueryFilters()
                .Where(w => w.StoreId == store.Id && w.Id == request.WarehouseId)
                .SelectMany(w => w.Products)
                .AsQueryable();

            if (!await query.AnyAsync(cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> {
                    "Current user does not owen a products for this warehouse." });
            }

            if (!string.IsNullOrEmpty(request.CodeNumber))
            {
                query = query.Where(w => w.CodeNumber.Equals(request.CodeNumber));
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(w => w.ProductTranslations.Any(pt => pt.Name.Equals(request.Name)));
            }

            if (request.IsInStockStatus != null)
            {
                query = query.Where(w => w.IsInStock == request.IsInStockStatus);
            }

            query = request.OrderBy switch
            {
                "priceAsc" => query.OrderBy(p => p.SellingPrice),
                "priceDesc" => query.OrderByDescending(p => p.SellingPrice),
                _ => query.OrderByDescending(p => p.DateAdded)
            };

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            var result = query.Select(p => new Response
            {
                CodeNumber = p.CodeNumber,
                Id = p.Id,
                ImageUrl = baseUri.CombineUri(p.Images
                    .OrderByDescending(p => p.DateUploaded)
                    .FirstOrDefault(p =>
                        p.Type == "ProductImage" && p.StoreId == store.Id && query.Any(item => item.Id == p.ProductId)).ImageUrl),
                Name = p.ProductTranslations.FirstOrDefault().Name,
                SellingPrice = p.SellingPrice,
                IsInStockStatus = p.IsInStock.Value
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
