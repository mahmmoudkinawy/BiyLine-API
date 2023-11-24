namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[ApiController]
[Route("api/v{version:apiVersion}/location")]
[Authorize]
public sealed class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("countries")]
    public async Task<ActionResult<IReadOnlyList<GetCountriesFeature.Response>>> GetCountries(
        [FromQuery] FilterParams filterParams)
    {
        var response = await _mediator.Send(new GetCountriesFeature.Request
        {
            Predicate = filterParams.Predicate,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{countryId}/governments")]
    public async Task<ActionResult<IReadOnlyList<GetGovernoratesFeature.Response>>> GetGovernorates(
        [FromRoute] int countryId,
        [FromQuery] FilterParams filterParams)
    {
        var response = await _mediator.Send(new GetGovernoratesFeature.Request
        {
            CountryId = countryId,
            Predicate = filterParams.Predicate,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{governorateId}/regions")]
    public async Task<ActionResult<IReadOnlyList<GetRegionsFeature.Response>>> GetRegions(
        [FromRoute] int governorateId,
        [FromQuery] FilterParams filterParams)
    {
        var response = await _mediator.Send(new GetRegionsFeature.Request
        {
            GovernorateId = governorateId,
            Predicate = filterParams.Predicate,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

}
