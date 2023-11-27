namespace BiyLineApi.Features.Users;
public sealed class SuspendEmployeeByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int EmployeeId { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            UserManager<UserEntity> userManager,
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == currentUserId, cancellationToken: cancellationToken);

            var employee = await _userManager.Users
                .Include(u => u.Employees)
                .FirstOrDefaultAsync(u =>
                    u.StoreId == store.Id && u.Employees.Any(e => e.Id
                     == request.EmployeeId && e.Id != currentUserId), cancellationToken: cancellationToken);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "Employee does not found" });
            }

            employee.LockoutEnabled = true;
            employee.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(employee);

            return Result<Response>.Success(new Response { });
        }
    }
}
