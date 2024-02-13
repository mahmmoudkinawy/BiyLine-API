namespace BiyLineApi.Features.ShippingCompanies
{
    public sealed class CompleteShippingCompanyDataFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
            public PaymentMethodEnum? PaymentMethod { get; set; }
            public string? PhoneNumber { get; set; }
            public DeliveryCases? DeliveryCases { get; set; }
            public int? Collection { get; set; }
        }

        public sealed class Response {
            public ShippingCompanyEntity ShippingCompanyEntity { get; set; }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
        //    public Validator(IStringLocalizer<CommonResources> localizer, BiyLineDbContext context)
        //    {
        //        RuleFor(r => r.Name)
        //            .NotEmpty()
        //            .MinimumLength(2)
        //            .MaximumLength(255);



        //        RuleFor(r => r.CountryCode)
        //            .NotEmpty()
        //                .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);

        //        RuleFor(r => r.Address)
        //            .NotEmpty()
        //                .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);
        //        RuleFor(r => r.IDImage)
        //.Must(IsValidImage)
        //    .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
        //        RuleFor(r => r.ProfileImage)
        //.Must(IsValidImage)
        //    .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
        //        RuleFor(r => r.CommercialRegisterImage)
        //.Must(IsValidImage)
        //    .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");

        //    }
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
            private readonly BiyLineDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IImageService _imageService;
            private readonly IDateTimeProvider _dateTimeProvider;

            public Handler(
                BiyLineDbContext context,
                UserManager<UserEntity> userManager,
                            IImageService imageService,
                                        IDateTimeProvider dateTimeProvider,

                IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager ??
        throw new ArgumentNullException(nameof(userManager));
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
                _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
                _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var shippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(s =>s.Id == request.Id);
                    
                    if(shippingCompany == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company not found" });
                    }
                    shippingCompany.Collection = request.Collection;

                    shippingCompany.PaymentMethod = request.PaymentMethod;
                    shippingCompany.PhoneNumber = request.PhoneNumber;
                    shippingCompany.DeliveryCases = request.DeliveryCases;
                   _context.ShippingCompanies.Update(shippingCompany);

                    await _context.SaveChangesAsync(cancellationToken);
                    return Result<Response>.Success(new Response {ShippingCompanyEntity = shippingCompany });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
