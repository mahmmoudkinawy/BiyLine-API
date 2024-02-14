namespace BiyLineApi.Features.Users;
public sealed class GetEmployeesForStoreByTraderFeature
{
    public sealed class Request : IRequest<Response>
    {
        public string? Predicate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public sealed class Response
    {
        public int TotalEmployees { get; set; }
        public decimal TotalSalary{ get; set; }
        public PagedList<Data> Data { get; set; }

    }
    public sealed class Data
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

    public sealed class Handler : IRequestHandler<Request, Response>
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == currentUserId, cancellationToken: cancellationToken);

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());

            var unfilteredQuery = _context.Employees
         .Include(e => e.User)
             .ThenInclude(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role)
         .Include(e => e.Permissions)
         .Where(u => u.StoreId == store.Id && u.UserId != currentUserId)
         .AsQueryable();

            var totalSalary = await unfilteredQuery.SumAsync(u => u.Salary);
            var totalEmployees = await unfilteredQuery.CountAsync();

            var query = unfilteredQuery;


            if (!string.IsNullOrEmpty(request.Predicate))
            {
                var searchTerm = request.Predicate.ToLower();

                query = query.Where(u =>
                    u.User.Email.ToLower().Contains(searchTerm) ||
                    u.User.UserName.ToLower().Contains(searchTerm) ||
                    u.User.Name.ToLower().Contains(searchTerm) ||
                    u.User.UserRoles.Any(r => r.Role.Name.ToLower().Contains(searchTerm)));
            }


            var result = query.Select(user => new Data
            {
                Id = user.Id,
                LastLogIn = user.User.LastActive.Value,
                Email = user.User.Email,
                Name = user.User.Name,
                Username = user.User.UserName,
                Roles = user.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Salary = user.User.Employees.FirstOrDefault(u => u.Id == user.Id).Salary.Value,
                PhoneNumber = user.User.PhoneNumber,
                EmploymentDate = user.EmploymentDate,
                Address = user.Address,
                Permissions = user.Permissions.Select(p => p.PermissionName).ToList(),
                
            });

            
            var employees =  await PagedList<Data>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);

            return new Response
            {
                TotalEmployees = totalEmployees,
                TotalSalary = totalSalary.Value,
                Data = employees
            };
        }
    }
}
