namespace BiyLineApi.Features.Stores;
public sealed class GetCurrentStoreProductsFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public bool? IsInStock { get; set; }
        public string? Name { get; set; }
        public string? CodeNumber { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeNumber { get; set; }
        public bool IsInStock { get; set; }
        public decimal SellingPrice { get; set; }
        public string ImageUrl { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var query = _context.Products
                .Include(p => p.ProductTranslations)
                .Where(p => p.StoreId == store.Id)
                .OrderByDescending(p => p.DateAdded)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(p => p.ProductTranslations.Any(pt => pt.Name.Contains(request.Name)));
            }

            if (!string.IsNullOrWhiteSpace(request.CodeNumber))
            {
                query = query.Where(p => p.CodeNumber == request.CodeNumber);
            }

            if (request.IsInStock != null)
            {
                query = query.Where(p => p.IsInStock == request.IsInStock);
            }

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            var products = query.Select(product => new Response
            {
                Id = product.Id,
                SellingPrice = product.SellingPrice.Value,
                CodeNumber = product.CodeNumber,
                IsInStock = product.IsInStock.Value,
                Name = product.ProductTranslations.FirstOrDefault().Name,
                ImageUrl = baseUri.CombineUri(
                    product.Images.OrderByDescending(d => d.DateUploaded).FirstOrDefault().ImageUrl)
            });

            return await PagedList<Response>.CreateAsync(
                products.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}
