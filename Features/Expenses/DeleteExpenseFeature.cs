namespace BiyLineApi.Features.Expenses;
public sealed class DeleteExpenseFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ExpenseId { get; set; }
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

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var expenseFromDb = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == request.ExpenseId && e.StoreId == store.Id);

            if (expenseFromDb is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this expense" });
            }

            _context.Expenses.Remove(expenseFromDb);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
