
namespace BiyLineApi.Features.Supplier;

public sealed class AddInsideSupplierFeature
{
    public sealed class Request : IRequest<Result<Response>> { }
    public sealed class Response { }
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                            throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                            throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var supplierId = _httpContextAccessor.GetValueFromRoute("userId");

            var supplierFromDb = await _context.Users.FirstOrDefaultAsync(s=>s.Id == supplierId);

            if (supplierFromDb is null)
            {
                return Result<Response>.Failure("This Supplier Is Not Found");
            }

            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return Result<Response>.Failure("This Store Is Not Found");
            }

            var supplier = new SupplierEntity
            {
                UserId = supplierId,
                SupplierType = SupplierTypeEnum.Inside.ToString(),
                StoreId = store.Id
            };

            _context.Add(supplier);

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
