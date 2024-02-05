namespace BiyLineApi.Features.Account;
public sealed class CustomerRegisterFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
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
                    .WithMessage(localizer[CommonResources.NameIsRequired].Value)
                .MinimumLength(2)
                    .WithMessage(localizer[CommonResources.NameMinimumLength].Value)
                .MaximumLength(255)
                    .WithMessage(localizer[CommonResources.NameMaximumLength].Value);

            RuleFor(r => r.Email)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.EmailIsRequired].Value)
                .EmailAddress()
                    .WithMessage(localizer[CommonResources.EmailIsValid].Value);

            RuleFor(r => r.Password)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PasswordIsRequired].Value);
        }
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
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<CommonResources> _localizer;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IMailService mailService,
            IStringLocalizer<CommonResources> localizer,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _localizer = localizer ??
                throw new ArgumentNullException(nameof(localizer));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            if (await _userManager.Users
                    .AnyAsync(u => u.Email == request.Email, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(
                    _localizer[CommonResources.UserWithEmailAlreadyExists].Value);
            }

            var userToCreate = _mapper.Map<UserEntity>(request);

            var userCreationResult = await _userManager
                .CreateAsync(userToCreate, request.Password);

            if (!userCreationResult.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in userCreationResult.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(userToCreate, Constants.Roles.Customer);

            var tokenToVerify = await _userManager.GenerateEmailConfirmationTokenAsync(userToCreate);

            var data = new
            {
                username = userToCreate.Name ?? userToCreate.Email,
                verificationLink = $"https://biyline.vercel.app/account/verify-email?userId={userToCreate.Id}&token={WebUtility.UrlEncode(tokenToVerify)}" // will be replaced
            };

            var emailTemplate = _mailService.LoadEmailTemplate("VerificationEmailTemplate.html");
            var emailBody = _mailService.RenderEmailTemplate(emailTemplate, data);

            // Will be replaced with a good mail design and good topics.
            await _mailService.SendEmailAsync(userToCreate.Email, "Email Confirmation", emailBody);

            return Result<Response>.Success(new Response
            {
                Message = _localizer[CommonResources.UserCheckMailForVerification].Value
            });
        }
    }

}
