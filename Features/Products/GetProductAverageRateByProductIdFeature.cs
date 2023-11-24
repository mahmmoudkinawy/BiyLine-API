namespace BiyLineApi.Features.Products;
public sealed class GetProductAverageRateByProductIdFeature
{
    public sealed class Request : IRequest<Response>
    {
        public int ProductId { get; set; }
    }

    public sealed class Response
    {
        public decimal? AverageRating { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly BiyLineDbContext _context;

        public Handler(BiyLineDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var averageRating = await _context.Rates
                .Where(r => r.ProductId == request.ProductId)
                .AverageAsync(r => r.Rating, cancellationToken: cancellationToken);

            return new Response
            {
                AverageRating = averageRating
            };
        }
    }

}
