namespace BiyLineApi.Features.Users;
public sealed class GetEmployeesForStoreByTraderFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public string? Predicate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public decimal Salary { get; set; }
        public List<string> Roles { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == currentUserId, cancellationToken: cancellationToken);

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());

            var query = _context.Employees
                .Include(e => e.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Where(u => u.StoreId == store.Id && u.UserId != currentUserId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                var searchTerm = request.Predicate.ToLower();

                query = query.Where(u =>
                    u.User.Email.ToLower().Contains(searchTerm) ||
                    u.User.UserName.ToLower().Contains(searchTerm) ||
                    u.User.Name.ToLower().Contains(searchTerm) ||
                    u.User.UserRoles.Any(r => r.Role.Name.ToLower().Contains(searchTerm)));
            }

            var result = query.Select(user => new Response
            {
                Id = user.Id,
                Email = user.User.Email,
                Name = user.User.Name,
                Username = user.User.UserName,
                Roles = user.User.UserRoles.Select(ur => ur.Role.Name).ToList(),
                Salary = user.User.Employees.FirstOrDefault(u => u.Id == user.Id).Salary.Value
            });

            return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
