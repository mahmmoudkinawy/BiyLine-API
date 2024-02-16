using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.Security;

namespace BiyLineApi.Features.employees;
public sealed class UpdateEmployeeForStoreByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public decimal? Salary { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public List<string> Permissions { get; set; }

    }

    public sealed class Response
    {
        public string Message { get; set; }
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
            UserManager<UserEntity> employeeManager,
            RoleManager<RoleEntity> roleManager)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = employeeManager ?? throw new ArgumentNullException(nameof(employeeManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentemployeeId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == currentemployeeId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current trader does not have store" });
            }

            var employeeId = _httpContextAccessor.GetValueFromRoute("employeeId");

            var employee = await _context.Employees
                .Include(e => e.Permissions)
                .Include(e=>e.SalaryPayments)
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.StoreId == store.Id);

            if (employee == null )
            {
                return Result<Response>.Failure(new List<string>
                    { "employee not found or not associated with the current store" });
            }


            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == employee.UserId, cancellationToken: cancellationToken);

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            employee.Address = request.Address;
            employee.Salary = request.Salary;
            user.UserName = request.Username;
            user.Email = request.Email;
            
            foreach(var salaryPayment in employee.SalaryPayments)
            {
                salaryPayment.PaidAmount = request.Salary;
            }
            
            employee.Permissions.Clear();

            var claims = new List<Claim>();

            foreach (var permissionName in request.Permissions)
            {
                var existingPermission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == permissionName);
                if (existingPermission == null)
                {
                    existingPermission = new PermissionEntity
                    {
                        PermissionName = permissionName
                    };
                    _context.Permissions.Add(existingPermission);
                }
                employee.Permissions.Add(existingPermission);

                claims.Add(new Claim("Permission", permissionName));
            }

            var existingClaims = (await _userManager.GetClaimsAsync(user)).Where(c => c.Type == "Permission");

            await _userManager.RemoveClaimsAsync(user,existingClaims);


            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists || !IsExcludedRole(request.Role))
            {
                return Result<Response>.Failure(new List<string> { "Role does not exist or is excluded" });
            }

            await _userManager.RemoveFromRoleAsync(user, request.Role);

            var updateResult = await _userManager.UpdateAsync(user);

            await _userManager.AddToRoleAsync(user, request.Role);

            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(error => error.Description).ToList();
                return Result<Response>.Failure(errors);
            }

            await _context.SaveChangesAsync();
            return Result<Response>.Success(new Response { });
        }

        private static bool IsExcludedRole(string role)
        {
            var excludedRoles = new List<string> {
                Constants.Roles.Admin, Constants.Roles.Representative, Constants.Roles.Trader, Constants.Roles.Customer, Constants.Roles.Employee };
            return excludedRoles.Contains(role);
        }
    }
}
