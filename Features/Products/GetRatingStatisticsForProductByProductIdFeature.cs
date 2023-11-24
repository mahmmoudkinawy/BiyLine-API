namespace BiyLineApi.Features.Products;
public sealed class GetRatingStatisticsForProductByProductIdFeature
{
    public sealed class Request : IRequest<Response>
    {
        public int ProductId { get; set; }
    }

    public sealed class Response
    {
        public int OneStar { get; set; }
        public int TwoStars { get; set; }
        public int ThreeStars { get; set; }
        public int FourStars { get; set; }
        public int FiveStars { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly BiyLineDbContext _context;

        public Handler(BiyLineDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Response> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var ratings = await _context.Rates
                .Where(r => r.ProductId == request.ProductId)
                .Select(r => r.Rating)
                .ToListAsync(cancellationToken: cancellationToken);

            var roundedRatings = ratings.Select(r => Math.Ceiling(r.Value)).ToList();

            var ratingCounts = roundedRatings
                .GroupBy(r => r)
                .ToDictionary(g => g.Key, g => g.Count());

            return new Response
            {
                OneStar = ratingCounts.GetValueOrDefault(1, 0),
                TwoStars = ratingCounts.GetValueOrDefault(2, 0),
                ThreeStars = ratingCounts.GetValueOrDefault(3, 0),
                FourStars = ratingCounts.GetValueOrDefault(4, 0),
                FiveStars = ratingCounts.GetValueOrDefault(5, 0)
            };
        }
    }
}
