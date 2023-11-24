namespace BiyLineApi.Features.Users;
public sealed class GetStoreEmployeesSalaryCalculationFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public decimal TotalSalaries { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var employeesSalaries = await _context.Employees
                .Where(e => e.StoreId == store.Id && e.UserId != userId)
                .SumAsync(e => e.Salary.GetValueOrDefault(), cancellationToken);

            return new Response
            {
                TotalSalaries = employeesSalaries
            };
        }
    }
}
