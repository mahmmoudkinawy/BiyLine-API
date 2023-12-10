namespace BiyLineApi.Features.SalaryPayment;

public sealed class CreateSalaryPaymentFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public decimal Amount { get; set; }

        public DateTime? Date { get; set; }

        public int StoreWalletId { get; set; }

        public string? Note { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.Amount)
                .GreaterThan(0);

            RuleFor(s => s.StoreWalletId)
                .GreaterThan(0);
        }
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
            var employeeId = _httpContextAccessor.GetValueFromRoute("employeeId");

            var employee = await _context.Employees.FirstOrDefaultAsync(e=>e.Id==employeeId);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "this employee not found" });
            }

            var userId = _httpContextAccessor.GetUserById();

            var role = _httpContextAccessor.GetUserRole();

            StoreWalletEntity? storeWallet = null;

            if (role == Constants.Roles.Trader)
            {
                storeWallet = await _context.StoreWallets
                  .Include(s => s.Store)
                  .ThenInclude(s=>s.Employees)
                  .FirstOrDefaultAsync(s => s.Id == request.StoreWalletId && s.Store.OwnerId == userId && s.Store.Employees.Any(s=>s.Id==employeeId));

                if (storeWallet == null)
                {
                    return Result<Response>.Failure(new List<string> { "this store wallet not found" });
                }
            }
            else if (role == Constants.Roles.Employee)
            {
                storeWallet = await _context.StoreWallets
                   .Include(s => s.Store)
                   .ThenInclude(s => s.Employees)
                   .FirstOrDefaultAsync(s => s.Id == request.StoreWalletId && s.Employee.User.Id == userId && s.EmployeeId == s.Employee.Id && s.Store.Employees.Any(s => s.Id == employeeId));

                if (storeWallet == null)
                {
                    return Result<Response>.Failure(new List<string> { "this store wallet not found" });
                }
            }

            var salaryPayment = new SalaryPaymentEntity
            {
                Amount = request.Amount,
                Note = request.Note,
                EmployeeId = employeeId,
                StoreWalletId = request.StoreWalletId,
                Date = request.Date ?? DateTime.UtcNow,
            };

            _context.SalaryPayments.Add(salaryPayment);

            if (storeWallet.TotalBalance < request.Amount)
            {
                return Result<Response>.BadRequest("invalid operation");
            }

            storeWallet.TotalBalance -= request.Amount;
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }

}
