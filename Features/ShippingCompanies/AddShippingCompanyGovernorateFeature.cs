namespace BiyLineApi.Features.ShippingCompanies
{
    public sealed class AddShippingCompanyGovernorateFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public string Name { get; set; }
            public decimal ShippingCost { get; set; }
            public decimal PickUpCost { get; set; }
            public decimal ReturnCost { get; set; }
            public double Weight { get; set; }
            public decimal OverweightFees { get; set; }

            public int ShippingCompanyId { get; set; }
        }

        public sealed class Response
        {
            public ShippingCompanyGovernorateDetailsEntity ShippingCompanyGovernorate { get; internal set; }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator(IStringLocalizer<CommonResources> localizer, BiyLineDbContext context)
            {
                RuleFor(r => r.Name)
                    .NotEmpty()
                    .MinimumLength(2)
                    .MaximumLength(255);
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
                    var shippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(s => s.Id == request.ShippingCompanyId);

                    if (shippingCompany == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company not found" });
                    }
                    if(shippingCompany.PaymentMethod == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have Payment Method first" });
                    }
                    if(shippingCompany.PhoneNumber == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have Phone Number first" });
                    }
                    if(shippingCompany.DeliveryCases == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have delivery case first" });
                    }
                    var shippingCompanyGovernorate = new ShippingCompanyGovernorateDetailsEntity
                    {
                        ShippingCompanyId = request.ShippingCompanyId,
                        Name = request.Name,
                        OverweightFees = request.OverweightFees,
                        PickUpCost = request.PickUpCost,
                        ReturnCost = request.ReturnCost,
                        ShippingCost = request.ShippingCost,
                        Status = true,
                        Weight = request.Weight
                    };
                    await _context.ShippingCompanyGovernorateDetails.AddAsync(shippingCompanyGovernorate);

                    await _context.SaveChangesAsync(cancellationToken);
                    return Result<Response>.Success(new Response { ShippingCompanyGovernorate = shippingCompanyGovernorate });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
