namespace BiyLineApi.Features.Products.Commands;
public sealed class SetRecentViewedProductsFeature
{
    public sealed class Request : IRequest
    {
        public int ProductId { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task Handle(Request request, CancellationToken cancellationToken)
        {
            var recentViews = _httpContextAccessor.HttpContext.Request.Cookies["recentViews"];
            var productIds = string.IsNullOrEmpty(recentViews) ?
                new List<int>()
                :
                recentViews.Split(',').Select(int.Parse).ToList();

            productIds = productIds.Distinct().ToList();

            productIds.Insert(0, request.ProductId);

            int maxRecentViews = 15;

            if (productIds.Count > maxRecentViews)
            {
                productIds = productIds.Take(maxRecentViews).ToList();
            }

            _httpContextAccessor.HttpContext.Response.Cookies.Append("recentViews",
                string.Join(",", productIds), new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7)
                });

            return Task.CompletedTask;
        }
    }

}
