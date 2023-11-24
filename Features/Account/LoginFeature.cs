namespace BiyLineApi.Features.Account;
public sealed class LoginFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public sealed class Response
    {
        public bool IsConfirmed { get; set; } = true;
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer)
        {
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

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IStringLocalizer<CommonResources> _localizer;
        private readonly ITokenService _tokenService;

        public Handler(
            UserManager<UserEntity> userManager,
            IStringLocalizer<CommonResources> localizer,
            ITokenService tokenService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _localizer = localizer ??
                throw new ArgumentNullException(nameof(localizer));
            _tokenService = tokenService ??
                throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result<Response>.Failure(
                    _localizer[CommonResources.UserInvalidCredentials].Value);
            }

            if (!user.EmailConfirmed)
            {
                return Result<Response>.Success(new Response
                {
                    IsConfirmed = false,
                    Email = user.Email
                });
            }

            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!checkPasswordResult)
            {
                return Result<Response>.Failure(
                    _localizer[CommonResources.UserInvalidCredentials].Value);
            }

            return Result<Response>.Success(new Response
            {
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }
    }

}
