namespace BiyLineApi.Features.Supplier;
public sealed class GetSupplierByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int SupplierId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? TradeName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SupplierType { get; set; }
        public bool IsSuspended { get; set; }
    }

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
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return Result<Response>.Failure("This Store Is Not Found");
            }

            var supplierFromDb = await _context.Suppliers.Where(s => s.StoreId == store.Id)
                .Include(s => s.User)
                .ThenInclude(s => s.Store)
                .FirstOrDefaultAsync(s => s.Id == request.SupplierId);

            if (supplierFromDb == null)
            {
                return Result<Response>.Failure("This Supplier Is Not Found");
            }

            var response = new Response
            {
                Id = supplierFromDb.Id,
                Name = supplierFromDb.Name ?? supplierFromDb.User.Name,
                TradeName = supplierFromDb.TradeName ?? supplierFromDb.User.Store.EnglishName,
                PhoneNumber = supplierFromDb.PhoneNumber ?? supplierFromDb.User.PhoneNumber,
                AccountNumber = supplierFromDb.AccountNumber,
                PaymentMethod = supplierFromDb.PaymentMethod,
                SupplierType = supplierFromDb.SupplierType,
                IsSuspended = supplierFromDb.IsSuspended,
            };

            return Result<Response>.Success(response);
        }
    }
}
