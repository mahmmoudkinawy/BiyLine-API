namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}/shippingCompanies")]
[EnsureStoreProfileCompleteness]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[ApiController]
public sealed class ShippingCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShippingCompaniesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
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

    [HttpPost]
    public async Task<IActionResult> CreateShippingCompany(
        [FromBody] CreateShippingCompanyFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
}
