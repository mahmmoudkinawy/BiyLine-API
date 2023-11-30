namespace BiyLineApi.Features.Images;
public sealed class RemoveProductImageByProductIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ImageId { get; set; }
    }

    public sealed class Response { }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(p => p.StoreId == store.Id && p.Id == request.ImageId,
                    cancellationToken: cancellationToken);

            if (image is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this image" });
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
