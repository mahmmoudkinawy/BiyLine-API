using BiyLineApi.Features.employees;

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
        var response = await _mediator.Send(new GetCurrentUserFeature.Request());

        return Ok(response);
    }

    [ApiVersion(2.0)]
    [HttpPost("employee-for-current-store")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateEmployeeForStoreByTrader(
        [FromForm] CreateEmployeerForStoreByTraderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [ApiVersion(2.0)]
    [HttpPut("employee-for-current-store/{employeeId}")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<IActionResult> UpdateEmployeeForStoreByTrader(
        [FromRoute] int employeeId,
        [FromForm] UpdateEmployeeForStoreByTraderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [ApiVersion(2.0)]
    [HttpGet("employees-for-current-store/{employeeId}")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<ActionResult<GetEmployeeForStoreByTraderByEmployeeIdFeature.Response>> GetEmployeeForStoreByTraderByEmployeeIdFeature(
       [FromRoute] int employeeId)
    {
        var response = await _mediator.Send(new GetEmployeeForStoreByTraderByEmployeeIdFeature.Request
        {
            EmployeeId = employeeId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [ApiVersion(2.0)]
    [HttpGet("employees-for-current-store")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<ActionResult<IReadOnlyList<GetEmployeesForStoreByTraderFeature.Response>>> GetEmployeesForStoreByTrader(
        [FromQuery] EmployeeParams employeeParams)
    {
        var response = await _mediator.Send(new GetEmployeesForStoreByTraderFeature.Request
        {
            PageNumber = employeeParams.PageNumber,
            PageSize = employeeParams.PageSize,
            Predicate = employeeParams.Predicate
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

}
