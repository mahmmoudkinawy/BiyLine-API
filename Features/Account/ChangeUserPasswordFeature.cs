namespace BiyLineApi.Features.Account;
public sealed class ChangeUserPasswordFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.CurrentPassword)
                .NotEmpty();

            RuleFor(r => r.NewPassword)
                .NotEmpty()
                .MinimumLength(6);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

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
                Message = "Your password changed successfully"
            });
        }
    }
}
