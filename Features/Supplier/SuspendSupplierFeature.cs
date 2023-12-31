﻿namespace BiyLineApi.Features.Supplier;
public sealed class SuspendSupplierFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int SupplierId { get; set; }
    }

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

            supplierFromDb.IsSuspended = true;
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
