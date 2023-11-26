namespace BiyLineApi.Controllers;

[ApiController]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/employees")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
public sealed class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
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

    [HttpPut("{employeeId}")]
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

    [HttpGet("current-store-employee/{employeeId}")]
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

    [HttpGet("current-store-employees")]
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
