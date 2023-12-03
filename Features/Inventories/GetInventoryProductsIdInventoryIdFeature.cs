namespace BiyLineApi.Features.Inventories;
public sealed class GetInventoryProductsIdInventoryIdFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public string? Predicate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string CodeNumber { get; set; }
        public string ImageUrl { get; set; }
        public int CountInStock { get; set; }
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
              .FirstOrDefaultAsync(s => s.StoreId == store.Id && s.Id == warehouseId,
                cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not this warehouse store." });
            }

            var query = _context.Inventories
                .Where(i => i.WarehouseId == warehouseId)
                .SelectMany(p => p.Warehouse.Products)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(p => p.ProductTranslations.Any(pt => pt.Name.Contains(request.Predicate)));
            }

            var result = query.Select(r => new Response
            {
                Id = r.Id,
                CodeNumber = r.CodeNumber,
                CountInStock = r.CountInStock.Value,
                Quantity = r.CountInStock.Value,
                ImageUrl = r.Images.FirstOrDefault(i => i.ProductId == r.Id && i.Type == "ProductImage" && i.StoreId == store.Id).ImageUrl
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
