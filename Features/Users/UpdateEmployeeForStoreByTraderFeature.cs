namespace BiyLineApi.Features.employees;
public sealed class UpdateEmployeeForStoreByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? Address { get; set; }
        public IFormFile? NationalIdImage { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public int? WorkingHours { get; set; }
        public decimal? Salary { get; set; }
        public string? PaymentPeriod { get; set; }
        public string? PaymentMethod { get; set; }
        public string? VisaNumber { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
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

            RuleFor(r => r.CountryCode)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PhoneNumberIsRequired].Value)
                .Must((req, phoneNumber, context) => IsPhoneNumberValid(phoneNumber, req.CountryCode))
                    .WithMessage(localizer[CommonResources.PhoneNumberIsValid].Value);

            RuleFor(r => r.Address)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(r => r.NationalIdImage)
                .Must(IsValidImage)
                    .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");

            RuleFor(r => r.EmploymentDate)
                .NotNull()
                .NotEmpty();

            RuleFor(r => r.WorkingHours)
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(100);

            RuleFor(r => r.Salary)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.PaymentPeriod)
                .Must(value => value != null && Enum.IsDefined(typeof(PaymentPeriodEnum), value))
                    .WithMessage("Invalid PaymentPeriod value. Available options are: Monthly, Weekly, TwoWeeks, Yearly.");

            RuleFor(r => r.PaymentMethod)
                .Must(value => value != null && Enum.IsDefined(typeof(PaymentMethodEnum), value))
                    .WithMessage("Invalid PaymentMethod value. Available options are: BankTransfer, Cache.");

            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Role)
                .NotEmpty();
        }

        private static bool IsValidImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return false;
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                return false;
            }

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName);
            return validExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsPhoneNumberValid(string phoneNumber, string countryCode)
        {
            try
            {
                var phoneUtility = PhoneNumberUtil.GetInstance();
                var phoneNumberObj = phoneUtility.Parse(phoneNumber, countryCode);
                var result = phoneUtility.IsValidNumber(phoneNumberObj) &&
                    phoneUtility.IsValidNumberForRegion(phoneNumberObj, countryCode);
                return result;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;
        private readonly BiyLineDbContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;

        public Handler(
            IHttpContextAccessor httpContextAccessor,
            IImageService imageService,
            BiyLineDbContext context,
            UserManager<UserEntity> employeeManager,
            RoleManager<RoleEntity> roleManager)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
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
                .Include(e => e.NationalIdImage)
                .FirstOrDefaultAsync(e => e.UserId == employeeId);

            if (employee == null || employee.StoreId != store.Id)
            {
                return Result<Response>.Failure(new List<string>
                    { "employee not found or not associated with the current store" });
            }

            if (request.NationalIdImage != null)
            {
                var imageUrl = await _imageService.UploadImageAsync(request.NationalIdImage, "EmployeesNationalIds");
                employee.NationalIdImage.ImageUrl = imageUrl;
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == employee.UserId, cancellationToken: cancellationToken);

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.CountryCode = request.CountryCode;
            employee.Address = request.Address;
            employee.EmploymentDate = request.EmploymentDate;
            employee.WorkingHours = request.WorkingHours;
            employee.Salary = request.Salary;
            employee.PaymentPeriod = Enum.TryParse<PaymentPeriodEnum>(request.PaymentPeriod, out var paymentPeriodEnum)
                ? paymentPeriodEnum
                : throw new ArgumentException("Invalid PaymentPeriod value.");
            employee.PaymentMethod = Enum.TryParse<PaymentMethodEnum>(request.PaymentMethod, out var paymentMethodEnum)
                ? paymentMethodEnum
                : throw new ArgumentException("Invalid PaymentMethod value.");
            employee.VisaNumber = request.VisaNumber;
            user.UserName = request.Username;
            user.Email = request.Email;

            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists || IsExcludedRole(request.Role))
            {
                return Result<Response>.Failure(new List<string> { "Role does not exist or is excluded" });
            }

            var isInRequestedRole = await _userManager.IsInRoleAsync(user, request.Role);

            if (!isInRequestedRole)
            {
                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        var errors = removeRolesResult.Errors.Select(error => error.Description).ToList();
                        return Result<Response>.Failure(errors);
                    }
                }

                var updateResult = await _userManager.UpdateAsync(user);

                await _userManager.AddToRoleAsync(user, request.Role);

                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(error => error.Description).ToList();
                    return Result<Response>.Failure(errors);
                }
            }

            return Result<Response>.Success(new Response { });
        }

        private static bool IsExcludedRole(string role)
        {
            var excludedRoles = new List<string> {
                Constants.Roles.Admin, Constants.Roles.Representative, Constants.Roles.Trader, Constants.Roles.Customer };
            return excludedRoles.Contains(role);
        }
    }
}
