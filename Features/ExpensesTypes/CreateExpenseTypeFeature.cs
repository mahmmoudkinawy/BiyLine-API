namespace BiyLineApi.Features.ExpensesTypes;
public sealed class CreateExpenseTypeFeature
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
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor,
            IDateTimeProvider dateTimeProvider,
            BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
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

            var expenseTypeToCreate = new ExpenseTypeEntity
            {
                StoreId = store.Id,
                Type = request.Type,
                StoreWalletId = storeWalletId,
                Amount = request.Amount,
                Created = _dateTimeProvider.GetCurrentDateTimeUtc()
            };

            _context.ExpenseTypes.Add(expenseTypeToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
