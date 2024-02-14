
namespace BiyLineApi.Features.SalaryPayment;

public sealed class GetSalaryPaymentsForEmployeeFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public string EmployeeName { get; set; }
        public string JobRole { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? Date { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
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

        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeId = _httpContextAccessor.GetValueFromRoute("employeeId");

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "this employee not found" });
            }

            var userId = _httpContextAccessor.GetUserById();

            var role = _httpContextAccessor.GetUserRole();

            IQueryable<SalaryPaymentEntity> query = null;

            if (role == Constants.Roles.Trader)
            {
                query = _context.SalaryPayments
              .Where(s => s.EmployeeId == employeeId && s.StoreWallet.Store.OwnerId == userId && s.StoreWallet.Store.Employees.Any(s => s.Id == employeeId))
              .AsQueryable();

            }
            else if (role == Constants.Roles.Employee)
            {
                query = _context.SalaryPayments
                   .Where(s => s.EmployeeId == employeeId && s.StoreWallet.Employee.User.Id == userId && s.StoreWallet.Store.Employees.Any(s => s.Id == employeeId))
                   .AsQueryable();
            }


            var salaryPayments = query.Select(s => new Response
            {
                EmployeeName = s.Employee.User.Name,
                Salary = s.Employee.Salary,
                JobRole = null, // will be replace
                Date = s.PaymentDate
            });

            return await PagedList<Response>.CreateAsync(
                salaryPayments.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value
                );
        }
    }



}
