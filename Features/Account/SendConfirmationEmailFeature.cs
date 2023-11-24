namespace BiyLineApi.Features.Account;
public sealed class SendConfirmationEmailFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Email { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
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
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<CommonResources> _localizer;

        public Handler(
            UserManager<UserEntity> userManager,
            IMailService mailService,
            IStringLocalizer<CommonResources> localizer)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _localizer = localizer ??
                throw new ArgumentNullException(nameof(localizer));
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
                var tokenToVerify = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var data = new
                {
                    username = user.Name ?? user.Email,
                    verificationLink = $"http://localhost:4200/account/confirm-email?userId={user.Id}&token={WebUtility.UrlEncode(tokenToVerify)}" // will be replaced
                };

                var emailTemplate = _mailService.LoadEmailTemplate("ConfirmationEmailTemplate.html");
                var emailBody = _mailService.RenderEmailTemplate(emailTemplate, data);

                // Will be replaced with a good mail design and good topics.
                await _mailService.SendEmailAsync(user.Email, "Email Confirmation", emailBody);
            }

            return Result<Response>.Success(new Response
            {
                Message = _localizer[CommonResources.ConfirmationEmailSent].Value
            });
        }
    }
}
