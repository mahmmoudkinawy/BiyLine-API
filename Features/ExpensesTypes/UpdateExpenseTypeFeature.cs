namespace BiyLineApi.Features.ExpensesTypes;
public sealed class UpdateExpenseTypeFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Type { get; set; }
        public decimal Amount { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Type)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(255);

            RuleFor(e => e.Amount)
                .GreaterThanOrEqualTo(0);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor,
            BiyLineDbContext context)
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

            var storeWalletId = _httpContextAccessor.GetValueFromRoute("storeWalletId");

            if (!await _context.StoreWallets
                .AnyAsync(w => w.StoreId == store.Id && w.Id == storeWalletId, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Store wallet does not exist." });
            }

            var expenseTypeFromDb = await _context.ExpenseTypes
                .FirstOrDefaultAsync(et => et.StoreId == store.Id && et.Id == storeWalletId, 
                    cancellationToken: cancellationToken);

            if (expenseTypeFromDb is null)
            {
                return Result<Response>.Failure(new List<string> { "Current user does not owen this expense type." });
            }

            expenseTypeFromDb.Type = request.Type;
            expenseTypeFromDb.Amount = request.Amount;
            expenseTypeFromDb.StoreWalletId = storeWalletId;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
