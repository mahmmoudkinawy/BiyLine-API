using HandlebarsDotNet;
using Hangfire;

namespace BiyLineApi.Features.SalaryPayment;

public sealed class CreateSalaryPaymentFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int EmployeeId { get; set; }

    }

    public sealed class Response
    {

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

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "this employee not found" });
            }

            var userId = _httpContextAccessor.GetUserById();


            var salaryPayment = new SalaryPaymentEntity
            {
                EmployeeId = request.EmployeeId,
                PaymentDate = employee.EmploymentDate?.AddMonths(11),
                Status = SalaryPaymentEnum.Unpaid.ToString()
            };

            _context.SalaryPayments.Add(salaryPayment);

            await _context.SaveChangesAsync();


            RecurringJob.AddOrUpdate("job", () => CreateSalaryPayment(salaryPayment.EmployeeId), Cron.Monthly);

            return Result<Response>.Success(new Response { });
        }

        public async Task CreateSalaryPayment(int employeeId)
        {
            var latestSalaryPaymentForEmployee =await _context.SalaryPayments.Where(s => s.EmployeeId == employeeId).OrderByDescending(s => s.PaymentDate).FirstOrDefaultAsync();
            var salaryPayment = new SalaryPaymentEntity
            {

                EmployeeId = employeeId,
                PaymentDate = latestSalaryPaymentForEmployee.PaymentDate.Value.AddMonths(1),
                Status = SalaryPaymentEnum.Unpaid.ToString()
            };

            _context.SalaryPayments.Add(salaryPayment);
            await _context.SaveChangesAsync();

        }
    }
}