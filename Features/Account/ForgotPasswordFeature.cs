namespace BiyLineApi.Features.Account;
public sealed class ForgotPasswordFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Email { get; set; }
    }

    public sealed class Response
    {
        public bool IsConfirmed { get; set; } = true;
        public string Email { get; set; }
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
                return Result<Response>.Success(new Response
                {
                    IsConfirmed = false,
                    Message = _localizer[CommonResources.EmailConfirmationTokenSent].Value
                });
            }

            if (!user.EmailConfirmed)
            {
                return Result<Response>.Success(new Response
                {
                    IsConfirmed = false,
                    Email = user.Email
                });
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var data = new
            {
                username = user.Name ?? user.Email,
                resetLink = $"http://localhost:4200/account/reset-password?userId={user.Id}&token={WebUtility.UrlEncode(passwordResetToken)}" // will be replaced
            };

            var emailTemplate = _mailService.LoadEmailTemplate("PasswordForgotEmailTemplate.html");
            var emailBody = _mailService.RenderEmailTemplate(emailTemplate, data);

            // Will be replaced with a good mail design and good topics.
            await _mailService.SendEmailAsync(user.Email, "Forgot Password", emailBody);

            return Result<Response>.Success(new Response
            {
                IsConfirmed = false,
                Message = _localizer[CommonResources.EmailConfirmationTokenSent].Value
            });
        }
    }

}
