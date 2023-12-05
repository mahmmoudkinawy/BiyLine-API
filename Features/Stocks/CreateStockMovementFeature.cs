namespace BiyLineApi.Features.Stocks;
public sealed class CreateStockMovementFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Created { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(req => req.ProductId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(req => req.SourceWarehouseId)
               .NotEmpty()
               .GreaterThan(0);

            RuleFor(req => req.DestinationWarehouseId)
               .NotEmpty()
               .GreaterThan(0);

            RuleFor(req => req.DestinationWarehouseId)
                .Must((request, destinationWarehouseId) => destinationWarehouseId != request.SourceWarehouseId)
                .WithMessage("Source and Destination warehouses must be different");

            RuleFor(req => req.InvoiceNumber)
              .NotEmpty()
              .MaximumLength(255);

            RuleFor(req => req.Created)
              .NotEmpty()
              .NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
            }

            if (!await _context.Products
                .IgnoreQueryFilters()
                .AnyAsync(w => w.Id == request.ProductId && w.StoreId == store.Id, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Current store does not owen this product." });
            }

            if (!await _context.Warehouses
                .AnyAsync(w => w.Id == request.SourceWarehouseId && w.StoreId == store.Id, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Current store does not this Warehouse." });
            }

            if (!await _context.Warehouses
                .AnyAsync(w => w.Id == request.DestinationWarehouseId && w.StoreId == store.Id, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Current store does not this Warehouse." });
            }

            var stockToCreate = new StockEntity
            {
                Created = request.Created,
                DestinationWarehouseId = request.DestinationWarehouseId,
                InvoiceNumber = request.InvoiceNumber,
                SourceWarehouseId = request.SourceWarehouseId,
                StoreId = store.Id,
                ProductId = request.ProductId
            };

            _context.Stocks.Add(stockToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
