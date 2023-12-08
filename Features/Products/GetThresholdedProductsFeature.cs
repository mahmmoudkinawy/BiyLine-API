namespace BiyLineApi.Features.Products;
public sealed class GetThresholdedProductsFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductImageUrl { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierEmail { get; set; }
        public string? SupplierPhoneNumber { get; set; }
        public int Quantity { get; set; }
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
                .Where(p =>
                    p.CountInStock.Value <= p.ThresholdReached.Value &&
                    p.WarehouseId == warehouseId && p.StoreId == store.Id)
                .OrderBy(p => p.Id)
                .AsQueryable();

            if (!await query.AnyAsync(cancellationToken: cancellationToken))
            {
                return Result<Response>.Success(new Response { });
            }

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            var result = query.Select(p => new Response
            {
                Id = p.Id,
                Name = p.ProductTranslations.FirstOrDefault().Name,
                ProductImageUrl = baseUri.CombineUri(p.Images.OrderByDescending(i => i.DateUploaded)
                    .FirstOrDefault(i => i.ProductId.Value == p.Id && i.Type == "ProductImage")!.ImageUrl),
                Quantity = p.CountInStock.Value,
                SupplierEmail = p.Supplier != null ? p.Supplier.Email : null,
                SupplierName = p.Supplier != null ? p.Supplier.TradeName : null,
                SupplierPhoneNumber = p.Supplier != null ? p.Supplier.PhoneNumber : null,
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}