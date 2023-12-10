namespace BiyLineApi.Features.SalaryPayment;

public sealed class GetLastSalaryPaymentForEmployeeFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int EmployeeId { get; set; }
    }

    public sealed class Response
    {
        public string EmployeeName { get; set; }

        public string StoreWalletName { get; set; }

        public decimal? Salary { get; set; }

        public DateTime Date { get; set; }

        public string Notes { get; set; }

        public string SalaryPaymentStatus { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.EmployeeId)
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
            var employee = await _context.Employees.Include(e=>e.User).FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "this employee not found" });
            }

            var userId = _httpContextAccessor.GetUserById();

            var role = _httpContextAccessor.GetUserRole();

            SalaryPaymentEntity? salaryPayment = null;

            if (role == Constants.Roles.Trader)
            {
                salaryPayment = await _context.SalaryPayments
                  .Include(s=>s.StoreWallet)
                  .ThenInclude(s => s.Store)
                  .ThenInclude(s => s.Employees)
                  .OrderByDescending(s=>s.Date)
                  .FirstOrDefaultAsync(s => s.EmployeeId==request.EmployeeId && s.StoreWallet.Store.OwnerId == userId && s.StoreWallet.Store.Employees.Any(s => s.Id == request.EmployeeId));

                if (salaryPayment == null)
                {
                    return Result<Response>.Failure(new List<string> { "there is no salary payment for this employee" });
                }
            }
            else if (role == Constants.Roles.Employee)
            {
                salaryPayment = await _context.SalaryPayments

                   .Include(s=>s.StoreWallet)
                   .ThenInclude(s => s.Store)
                   .ThenInclude(s => s.Employees)
                   .Include(s => s.StoreWallet)
                   .ThenInclude(s=>s.Employee)
                   .OrderByDescending(s => s.Date)
                   .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId && s.StoreWallet.Employee.User.Id == userId &&  s.StoreWallet.Store.Employees.Any(s => s.Id == request.EmployeeId));

                if (salaryPayment == null)
                {
                    return Result<Response>.Failure(new List<string> { "there is no salary payment for this employee" });
                }
            }

            var response = new Response
            {
                EmployeeName = employee.User.Name,
                Salary = employee.Salary,
                Notes = salaryPayment.Note,
                Date = salaryPayment.Date,
                StoreWalletName = salaryPayment.StoreWallet.Name
            };

            if(employee.PaymentPeriod == PaymentPeriodEnum.Monthly.ToString())
                
            {
                if ((DateTime.UtcNow - response.Date) > new TimeSpan(30, 0, 0, 0))
                   response.SalaryPaymentStatus = SalaryPaymentEnum.Paid.ToString(); 
                else
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Unpaid.ToString();

            }
            else if(employee.PaymentPeriod == PaymentPeriodEnum.TwoWeeks.ToString() )
            {
                if ((DateTime.UtcNow - response.Date) > new TimeSpan(14, 0, 0, 0))
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Paid.ToString();
                else
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Unpaid.ToString();
            }
            else if(employee.PaymentPeriod == PaymentPeriodEnum.Weekly.ToString())
            {
                if ((DateTime.UtcNow - response.Date) > new TimeSpan(7, 0, 0, 0))
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Paid.ToString();
                else
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Unpaid.ToString();
            }
            else if(employee.PaymentPeriod == PaymentPeriodEnum.Yearly.ToString())
            {
                if ((DateTime.UtcNow - response.Date) > new TimeSpan(356, 0, 0, 0))
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Paid.ToString();
                else
                    response.SalaryPaymentStatus = SalaryPaymentEnum.Unpaid.ToString();
            }

            return Result<Response>.Success(response);  
        }
    }
}
