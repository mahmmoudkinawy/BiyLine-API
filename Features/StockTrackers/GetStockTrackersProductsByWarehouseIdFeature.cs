namespace BiyLineApi.Features.StockTrackers;
public sealed class GetStockTrackersProductsByWarehouseIdFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public string OrederBy { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            var warehouseId = _httpContextAccessor.GetValueFromRoute("warehouseId");

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.StoreId == store.Id && w.Id == warehouseId,
                    cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen warehouse." });
            }

            var query = _context.Products
                .Where(p => p.WarehouseId == warehouseId && p.StoreId == store.Id)
                .OrderBy(p => p.Id)
                .AsQueryable();

            query = request.OrederBy switch
            {
                "name" => query.OrderBy(p => p.ProductTranslations.FirstOrDefault().Name),
                "priceAsc" => query.OrderBy(p => p.OriginalPrice.Value),
                "priceDesc" => query.OrderByDescending(p => p.OriginalPrice.Value),
                _ => query
            };

            var result = query.Select(r => new Response
            {
                Id = r.Id,
                Quantity = r.CountInStock.Value,
                ImageUrl = r.Images.OrderByDescending(i => i.DateUploaded).FirstOrDefault(i => i.ProductId == r.Id && i.Type.Equals("ProductImage")).ImageUrl
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }

}
