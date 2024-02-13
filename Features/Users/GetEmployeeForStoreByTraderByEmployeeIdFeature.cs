namespace BiyLineApi.Features.Users;
public sealed class GetEmployeeForStoreByTraderByEmployeeIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int EmployeeId { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? LastLogIn { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public List<string>? Roles { get; set; }
        public List<string> Permissions { get; set; }

    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == currentUserId, cancellationToken: cancellationToken);

            var employee = await _context.Employees
                .Include(e => e.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Include(e=>e.Permissions)
                .FirstOrDefaultAsync(u => u.StoreId == store.Id && u.UserId != currentUserId && u.Id == request.EmployeeId, cancellationToken: cancellationToken);

            if (employee == null)
            {
                return Result<Response>.Failure(new List<string> { "Employee does not found" });
            }

            return Result<Response>.Success(new Response
            {
                Id = employee.Id,
                LastLogIn = employee.User?.LastActive.GetValueOrDefault(),
                Roles = employee.User?.UserRoles?.Select(ur => ur.Role.Name).ToList(),
                Salary = employee.User?.Employees?.FirstOrDefault(e => e.Id == request.EmployeeId)?.Salary,
                Username = employee.User?.UserName,
                Email = employee?.User?.Email,
                Name = employee?.User?.Name,
                Address = employee.Address,
                PhoneNumber = employee.User?.PhoneNumber,
                EmploymentDate = employee.EmploymentDate,
                Permissions = employee.Permissions.Select(p => p.PermissionName).ToList(),
            });

        }
    }
}
