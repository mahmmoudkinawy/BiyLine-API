namespace BiyLineApi.Features.Account;
public sealed class TraderRegisterFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CountryCode { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer)
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.NameIsRequired].Value)
                .MinimumLength(2)
                    .WithMessage(localizer[CommonResources.NameMinimumLength].Value)
                .MaximumLength(255)
                    .WithMessage(localizer[CommonResources.NameMaximumLength].Value);

            RuleFor(r => r.CountryCode)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PhoneNumberIsRequired].Value)
                .Must((req, phoneNumber, context) => IsPhoneNumberValid(phoneNumber, req.CountryCode))
                    .WithMessage(localizer[CommonResources.PhoneNumberIsValid].Value);

            RuleFor(r => r.Email)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.EmailIsRequired].Value)
                .EmailAddress()
                    .WithMessage(localizer[CommonResources.EmailIsValid].Value);

            RuleFor(r => r.Password)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PasswordIsRequired].Value);
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

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, UserEntity>()
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IStringLocalizer<CommonResources> _localizer;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IStringLocalizer<CommonResources> localizer,
            IMailService mailService,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _localizer = localizer ??
                throw new ArgumentNullException(nameof(localizer));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            if (await _userManager.Users
                    .AnyAsync(u => u.Email == request.Email, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    _localizer[CommonResources.UserWithEmailAlreadyExists].Value
                });
            }

            var traderToCreate = _mapper.Map<UserEntity>(request);

            var traderCreationResult = await _userManager
                .CreateAsync(traderToCreate, request.Password);

            if (!traderCreationResult.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in traderCreationResult.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(traderToCreate, Constants.Roles.Trader);

            var tokenToVerify = await _userManager.GenerateEmailConfirmationTokenAsync(traderToCreate);

            var data = new
            {
                username = traderToCreate.Name ?? traderToCreate.Email,
                verificationLink = $"http://localhost:4200/account/verify-email?userId={traderToCreate.Id}&token={WebUtility.UrlEncode(tokenToVerify)}" // will be replaced
            };

            var emailTemplate = _mailService.LoadEmailTemplate("VerificationEmailTemplate.html");
            var emailBody = _mailService.RenderEmailTemplate(emailTemplate, data);

            // Will be replaced with a good mail design and good topics.
            await _mailService.SendEmailAsync(traderToCreate.Email, "Email Confirmation", emailBody);

            return Result<Response>.Success(new Response
            {
                Message = _localizer[CommonResources.UserCheckMailForVerification].Value
            });
        }
    }

}
