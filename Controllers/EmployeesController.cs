
namespace BiyLineApi.Controllers;

[ApiController]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/employees")]
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
    [Authorize(Policy = Constants.Policies.EmployeeWrite)]
    public async Task<IActionResult> CreateEmployeeForStoreByTrader(
        [FromBody] CreateEmployeerForStoreByTraderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        var createSalaryPaymentRequest = new CreateSalaryPaymentFeature.Request { EmployeeId = response.Value.EmployeeId };

        var createSalaryPaymentResponse = await _mediator.Send(createSalaryPaymentRequest);

        if (!createSalaryPaymentResponse.IsSuccess)
        {
            return BadRequest(createSalaryPaymentResponse.Errors);
        }

        return NoContent();
    }

    [HttpPut("{employeeId}/suspend")]
    [Authorize(Policy = Constants.Policies.EmployeeWrite)]

    public async Task<IActionResult> SuspendEmployeeByTrader(
        [FromRoute] int employeeId)
    {
        var response = await _mediator.Send(new SuspendEmployeeByTraderFeature.Request
        {
            EmployeeId = employeeId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{employeeId}")]
    [Authorize(Policy = Constants.Policies.EmployeeWrite)]

    public async Task<IActionResult> UpdateEmployeeForStoreByTrader(
        [FromBody] UpdateEmployeeForStoreByTraderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("current-store-employee/{employeeId}")]
    [Authorize(Policy = Constants.Policies.EmployeeRead)]
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
    [Authorize(Policy = Constants.Policies.EmployeeRead)]

    public async Task<ActionResult<GetEmployeesForStoreByTraderFeature.Response>> GetEmployeesForStoreByTrader(
        [FromQuery] EmployeeParams employeeParams)
    {
        var response = await _mediator.Send(new GetEmployeesForStoreByTraderFeature.Request
        {
            PageNumber = employeeParams.PageNumber,
            PageSize = employeeParams.PageSize,
            Predicate = employeeParams.Predicate
        });

        Response.AddPaginationHeader(
            response.Data.CurrentPage,
            response.Data.PageSize,
            response.Data.TotalPages,   
            response.Data.TotalCount);


        return Ok(response);
    }

    [HttpPut("{employeeId}/change-password")]
    [Authorize(Policy =Constants.Policies.MustBeTrader)]
    public async Task<IActionResult> ChangeEmployeePassword([FromBody] ChangeEmployeePasswordByTraderFeature.Request request)
    {
        var response = await _mediator.Send(request);
        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }
        if(response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);

    }

   

}
