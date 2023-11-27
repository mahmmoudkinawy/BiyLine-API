namespace BiyLineApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [Authorize]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpGet("current-user")]
    public async Task<ActionResult<GetCurrentUserFeature.Response>> GetCurrentUser()
    {
        var response = await _mediator.Send(new GetCurrentUserFeature.Request { });

        return Ok(response);
    }
}
