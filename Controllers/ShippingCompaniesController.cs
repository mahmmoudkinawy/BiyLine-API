namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/shippingCompanies")]
//[EnsureStoreProfileCompleteness]
[ApiController]
[ApiVersion(3.0)]
[Authorize]
public sealed class ShippingCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShippingCompaniesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("GetShippingCompanies")]
    public async Task<ActionResult<IReadOnlyList<GetShippingCompaniesFeature.Response>>> GetShippingCompanies(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetShippingCompaniesFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

     
        Response.AddPaginationHeader(
             response.CurrentPage,
             response.PageSize,
             response.TotalPages,
             response.TotalCount);

        return Ok(response);
    }

    [HttpPost("test")]
    public async Task<IActionResult> CreateShippingCompany1(
       [FromForm] CreateShippingCompanyFeature.Request request)
    {
        return NoContent();
    }


    [HttpPost("CreateShippingCompany")]
    public async Task<IActionResult> CreateShippingCompany(
        [FromForm] CreateShippingCompanyFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPost("CompleteShippingCompany")]
    public async Task<IActionResult> CompleteShippingCompany(
        [FromBody] CompleteShippingCompanyDataFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
    
    [HttpPost("CreateShippingCompanyGovernorate")]
    public async Task<IActionResult> CreateShippingCompanyGovernorate(
        [FromBody] AddShippingCompanyGovernorateFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
    [HttpPost("UpdateShippingCompanyGovernorate")]
    public async Task<IActionResult> UpdateShippingCompanyGovernorate(
        [FromBody] UpdateShippingCompanyGovernorateDetailsFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
    [HttpPost("UpdateShippingCompanyGovernorateDetailsStatus")]
    public async Task<IActionResult> UpdateShippingCompanyGovernorateDetailsStatus(
        [FromBody] UpdateShippingCompanyGovernorateDetailsStatusFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
}
