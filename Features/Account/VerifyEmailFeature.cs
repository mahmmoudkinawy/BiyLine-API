namespace BiyLineApi.Features.Account;
public sealed class VerifyEmailFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? UserId { get; set; }
        public string? Token { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IStringLocalizer<CommonResources> _localizer;

        public Handler(
            UserManager<UserEntity> userManager,
            IStringLocalizer<CommonResources> localizer)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _localizer = localizer ??
                throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                return Result<Response>.Failure(
                    _localizer[CommonResources.UserWithEmailDoesNotExist].Value);
            }

            if (user.EmailConfirmed)
            {
                return Result<Response>.Failure(
                    _localizer[CommonResources.UserEmailAlreadyVerified].Value);
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
            }

            return Result<Response>.Success(new Response
            {
                Message = _localizer[CommonResources.UserEmailAlreadyVerified].Value
            });
        }
    }
}
