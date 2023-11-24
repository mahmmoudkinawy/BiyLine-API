namespace BiyLineApi.Features.Users;
public sealed class CreateEmployeerForStoreByTraderFeature
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
        public string? Password { get; set; }
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

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(6);

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
            UserManager<UserEntity> userManager,
            RoleManager<RoleEntity> roleManager)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
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

            var imageUrl = await _imageService.UploadImageAsync(request.NationalIdImage, "EmployeesNationalIds");

            var employeeToCreate = new UserEntity
            {
                StoreId = store.Id,
                Email = request.Email,
                UserName = request.Username,
                PhoneNumber = request.PhoneNumber,
                CountryCode = request.CountryCode,
                Name = request.Name,
                Employees = new List<EmployeeEntity>
                {
                    new EmployeeEntity
                    {
                        EmploymentDate = request.EmploymentDate,
                        Address = request.Address,
                        WorkingHours = request.WorkingHours,
                        VisaNumber = request.VisaNumber,
                        PaymentMethod = Enum.TryParse<PaymentMethodEnum>(request.PaymentMethod, out var paymentMethodEnum)
                                 ? paymentMethodEnum
                                 : throw new ArgumentException("Invalid PaymentMethod value."),
                        PaymentPeriod = Enum.TryParse<PaymentPeriodEnum>(request.PaymentPeriod, out var paymentPeriodEnum)
                                 ? paymentPeriodEnum
                                 : throw new ArgumentException("Invalid PaymentPeriod value."),
                        Salary = request.Salary,
                        NationalIdImage = new ImageEntity
                        {
                            ImageMimeType = request.NationalIdImage.ContentType,
                            FileName = request.NationalIdImage.FileName,
                            ImageUrl = imageUrl,
                            DateUploaded = DateTime.UtcNow,
                            Type = "EmployeesNationalIds"
                        }
                    }
                }
            };

            var roleExists = await _roleManager.RoleExistsAsync(request.Role);

            if (!roleExists || IsExcludedRole(request.Role))
            {
                return Result<Response>.Failure(new List<string> { "Role does not exist or is excluded" });
            }

            var result = await _userManager.CreateAsync(employeeToCreate);
            await _userManager.AddToRoleAsync(employeeToCreate, request.Role.Trim());
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
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
