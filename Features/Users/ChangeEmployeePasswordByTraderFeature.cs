using Microsoft.Identity.Client;

namespace BiyLineApi.Features.Users;

public sealed class ChangeEmployeePasswordByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string NewPassword { get; set; }
    }
    
    public sealed class Response
    {
        public string Message { get; set; }
    }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer)
        {
                 RuleFor(r => r.NewPassword)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;
        private readonly UserManager<UserEntity> _userManager;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context, UserManager<UserEntity> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeId = _httpContextAccessor.GetValueFromRoute("employeeId");
            var user =  await _userManager.FindByIdAsync(employeeId.ToString());
            if(user == null)
            {
                return Result<Response>.Failure(new List<string> { "this employee not found" });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
            {
                return Result<Response>.BadRequest("Couldn't update password, please try again");
            }

            return Result<Response>.Success(new Response { Message = "Password has changed successfully" });
        }
    }

}
