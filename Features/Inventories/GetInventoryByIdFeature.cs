namespace BiyLineApi.Features.Inventories;
public sealed class GetInventoryByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int InventoryId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string CodeNumber { get; set; }
        public DateTime Created { get; set; }
        public int WarehouseId { get; set; }
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

            var warehouseId = _httpContextAccessor.GetValueFromRoute("warehouseId");

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.StoreId == store.Id && w.Id == warehouseId,
                    cancellationToken: cancellationToken);

            if (warehouse is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not this warehouse." });
            }

            var inventoryId = _httpContextAccessor.GetValueFromRoute("inventoryId");

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.WarehouseId == warehouseId && i.Id == inventoryId,
                    cancellationToken: cancellationToken);

            if (inventory == null)
            {
                return Result<Response>.Failure(new List<string> { "Inventory does not exist." });
            }

            return Result<Response>.Success(new Response
            {
                Id = inventory.Id,
                CodeNumber = inventory.CodeNumber,
                Created = inventory.Created.Value,
                WarehouseId = inventory.WarehouseId
            });
        }
    }

}
