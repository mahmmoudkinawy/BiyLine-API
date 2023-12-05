namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetCategoriesFeature.Response>>> GetCategories(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetCategoriesFeature.Request
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

}
