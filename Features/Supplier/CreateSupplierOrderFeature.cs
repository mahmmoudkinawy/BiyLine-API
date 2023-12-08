namespace BiyLineApi.Features.Supplier;
public sealed class CreateSupplierOrderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? PaymentMethod { get; set; }
        public int Quantity { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Quantity)
                .GreaterThan(0);

            RuleFor(r => r.PaymentMethod)
                .NotEmpty()
                .Must(paymentMethod => Enum.TryParse<PaymentMethodEnum>(paymentMethod, out var _))
                    .WithMessage("Invalid PaymentMethod value. Available options are: BankTransfer, Cache.");
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

        public async Task<Result<Response>> Handle(
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

            var product = await _context.Products
                .FirstOrDefaultAsync(w =>
                    w.StoreId == store.Id && w.WarehouseId == warehouseId && w.Id == warehouseId,
                    cancellationToken: cancellationToken);

            if (product is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen this product." });
            }

            var supplierOrderToCreate = new ContractOrderEntity
            {
                FromStoreId = product.StoreId.Value,
                ToStoreId = store.Id,
                PaymentMethod = request.PaymentMethod,
                ContractOrderProducts = new List<ContractOrderProductEntity>
                {
                    new ContractOrderProductEntity
                    {
                        ProductId = product.Id,
                        ContractOrderVariations = product.ProductVariations.Select(pt => new ContractOrderVariationEntity
                        {
                            ProductVariationId = pt.Id,
                            Quantity = request.Quantity
                        }).ToList()
                    }
                }
            };

            _context.ContractOrders.Add(supplierOrderToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
