namespace BiyLineApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/account")]
public sealed class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpPost("login")]
    public async Task<ActionResult<LoginFeature.Response>> Login(
        [FromBody] LoginFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Error);
        }

        return Ok(response.Value);
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpPost("customer-register")]
    public async Task<ActionResult<CustomerRegisterFeature.Response>> CustomerRegister(
       [FromBody] CustomerRegisterFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

    //[ApiVersion(1.0)]
    //[HttpPost("register-with-google")]
    //public async Task<ActionResult<RegisterUserWithGoogleFeature.Response>> RegisterWithGoogle(
    //    [FromBody] RegisterUserWithGoogleFeature.Request request)
    //{
    //    var response = await _mediator.Send(request);

    //    if (!response.IsSuccess)
    //    {
    //        return BadRequest(response.Errors);
    //    }

    //    return Ok(response.Value);
    //}

    [ApiVersion(2.0)]
    [HttpPost("register-trader")]
    public async Task<ActionResult<RegisterStoreFeature.Response>> RegisterStoreTrader(
       [FromBody] RegisterStoreFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpGet("verify-email")]
    public async Task<ActionResult<VerifyEmailFeature.Response>> VerifyEmail(
       [FromQuery] VerifyEmailFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpPost("send-confirmation-email")]
    public async Task<IActionResult> SendConfirmationEmail(
       [FromBody] SendConfirmationEmailFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }


    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<ChangeUserPasswordFeature.Response>> ChangePassword(
        [FromBody] ChangeUserPasswordFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordFeature.Response>> ForgotPassword(
        [FromBody] ForgotPasswordFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }
}
