namespace BiyLineApi.Features.SalaryPayment;

public sealed class GetAllEmployeesWithLastSalaryPaymentFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public string? Predicate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
  
    public sealed class Response
    {
        public string EmployeeName { get; set; }

        public string JobRole { get; set; }

        public decimal? Salary {get; set; } 

        public DateTime? Date { get; set; }

    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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
        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var storeWalletId = _httpContextAccessor.GetValueFromRoute("storeWalletId");
            var userId = _httpContextAccessor.GetUserById();
            var role = _httpContextAccessor.GetUserRole();

            IQueryable<SalaryPaymentEntity> query = null;

            if (role == Constants.Roles.Trader)
            {
                query = _context.SalaryPayments
                  .Where(s => s.StoreWalletId == storeWalletId && s.StoreWallet.Store.OwnerId == userId)
                  .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(s => s.Employee.User.Name.Contains(request.Predicate));
                }

            }
            else if (role == Constants.Roles.Employee)
            {
                query = _context.SalaryPayments
                   .Where(s => s.StoreWalletId == storeWalletId && s.StoreWallet.Employee.User.Id == userId && s.StoreWallet.EmployeeId == s.StoreWallet.Employee.Id)
                   .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(s => s.Employee.User.Name.Contains(request.Predicate));
                }

            }

              var employees = query
              .GroupBy(s => s.Employee.User.Name)
              .Select(group => new Response
              {
                  EmployeeName = group.Key,
                  JobRole = null,    // Replace it with the actual logic
                  Salary = group.First().Employee.Salary,
                  Date = group.Max(s => s.PaymentDate)
              });

            return await PagedList<Response>.CreateAsync(
              employees.AsNoTracking(),
              request.PageNumber.Value,
              request.PageSize.Value);
        }
    }
}
