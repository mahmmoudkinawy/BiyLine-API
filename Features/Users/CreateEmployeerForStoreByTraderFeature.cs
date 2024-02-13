using AutoMapper.Configuration.Conventions;

namespace BiyLineApi.Features.Users;
public sealed class CreateEmployeerForStoreByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public decimal? Salary { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public List<string> Permissions { get; set; }
    }

    public sealed class Response
    {
        public int EmployeeId { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer)
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(255);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PhoneNumberIsRequired].Value);
               

            RuleFor(r => r.Address)
                .NotEmpty()
                .MaximumLength(500);
            
            RuleFor(r => r.Salary)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(r => r.Role)
                .NotEmpty();

            RuleFor(r => r.Permissions)
                .NotEmpty();
        }

       
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;

        public Handler(
            IHttpContextAccessor httpContextAccessor,
            BiyLineDbContext context,
            UserManager<UserEntity> userManager,
            RoleManager<RoleEntity> roleManager)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current trader does not have store" });
            }


            var user = new UserEntity
            {
                StoreId = store.Id,
                Email = request.Email,
                UserName = request.Username,
                PhoneNumber = request.PhoneNumber,
                Name = request.Name,
                EmailConfirmed = true
            };

            var employeeToCreate = new EmployeeEntity
            {
                Address = request.Address,
                Salary = request.Salary,
                StoreId = store.Id,
                UserId = user.Id,
            };

                var claims = new List<Claim>();
            foreach (var permission in request.Permissions)
            {

                var existingPermission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == permission);
                if (existingPermission == null)
                {
                    existingPermission = new PermissionEntity
                    {
                        PermissionName = permission
                    };
                    _context.Permissions.Add(existingPermission);
                }
                employeeToCreate.Permissions.Add(existingPermission);

                claims.Add(new Claim("Permission", permission));
                
            }

            user.Employees.Add(employeeToCreate);

            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists || !IsExcludedRole(request.Role))
            {
                return Result<Response>.Failure(new List<string> { "Role does not exist or is excluded" });
            }

            var result = await _userManager.CreateAsync(user,request.Password);

            await _userManager.AddToRoleAsync(user, request.Role.Trim());

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
            }

            await _userManager.AddClaimsAsync(user, claims);


            return Result<Response>.Success(new Response { EmployeeId = employeeToCreate.Id});
        }

        private static bool IsExcludedRole(string role)
        {
            var excludedRoles = new List<string> {
                Constants.Roles.Admin, Constants.Roles.Representative, Constants.Roles.Trader, Constants.Roles.Customer , Constants.Roles.Employee,Constants.Roles.Manager };
            return excludedRoles.Contains(role);
        }
    }
}
