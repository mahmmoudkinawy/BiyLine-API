using BiyLineApi.DbContexts.Migrations;

namespace BiyLineApi.Features.SalaryPayment;

public sealed class UpdateSalaryPaymentFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public decimal? Increase { get; set; } // الزياده
        public decimal? Deduction { get; set; } // الخصم
        public string? Notes { get; set; } // الملاحظات
        public int StoreWalletId { get; set; }
    }

    public sealed class Response
    {

    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {

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

            var salaryPaymentRecordId = _httpContextAccessor.GetValueFromRoute("salaryPaymentRecordId");

            var salaryPaymentRecord = await _context.SalaryPayments
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == salaryPaymentRecordId && s.EmployeeId == employeeId);

            if (salaryPaymentRecord == null)
            {
                return Result<Response>.Failure(new List<string> { "this salary payment record  not found" });
            }

            var userId = _httpContextAccessor.GetUserById();

            var role = _httpContextAccessor.GetUserRole();

            StoreWalletEntity? storeWallet = null;

            if (role == Constants.Roles.Trader)
            {
                storeWallet = await _context.StoreWallets
                  .Include(s => s.Store)
                  .ThenInclude(s => s.Employees)
                  .FirstOrDefaultAsync(s => s.Id == request.StoreWalletId && s.Store.OwnerId == userId && s.Store.Employees.Any(s => s.Id == employeeId) );

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

            salaryPaymentRecord.PaidAmount = salaryPaymentRecord.Employee.Salary;
            salaryPaymentRecord.Status = SalaryPaymentEnum.Paid.ToString();
            salaryPaymentRecord.Increase = request.Increase;
            salaryPaymentRecord.Deduction = request.Deduction;
            salaryPaymentRecord.Notes = request.Notes;
            salaryPaymentRecord.StoreWalletId = request.StoreWalletId;

            decimal? total = 0;

            if(request.Increase != null && request.Deduction!=null)
            {
                total = salaryPaymentRecord.PaidAmount + salaryPaymentRecord.Increase - salaryPaymentRecord.Deduction;
            }
            else if (request.Increase != null && request.Deduction == null)
            {
                total = salaryPaymentRecord.PaidAmount + salaryPaymentRecord.Increase;
            }
            else if (request.Increase == null && request.Deduction != null)
            {
                total = salaryPaymentRecord.PaidAmount - salaryPaymentRecord.Deduction;
            }

            if (storeWallet.TotalBalance  < total )
            {
                return Result<Response>.BadRequest("invalid operation");
            }

            storeWallet.TotalBalance -= total;
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
