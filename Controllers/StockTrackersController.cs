namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/stockTrackers")]
[ApiVersion(2.0)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[ApiController]
public sealed class StockTrackersController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockTrackersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("{warehouseId}")]
    public async Task<IActionResult> CreateStockTracker(
        [FromRoute] int warehouseId,
        [FromBody] CreateStockTrackerForWarehouseFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{warehouseId}")]
    public async Task<ActionResult<IReadOnlyList<GetStockTrackersProductsByWarehouseIdFeature.Response>>> GetStockTrackerProducts(
        [FromRoute] int warehouseId,
        [FromQuery] StockTrackersParams stockTrackersParams)
    {
        var response = await _mediator.Send(new GetStockTrackersProductsByWarehouseIdFeature.Request
        {
            OrederBy = stockTrackersParams.OrderBy,
            PageNumber = stockTrackersParams.PageNumber,
            PageSize = stockTrackersParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetStockTrackersProductsByWarehouseIdFeature.Response>>>(
            successResponse =>
            {
                Response.AddPaginationHeader(
                    successResponse.CurrentPage,
                    successResponse.PageSize,
                    successResponse.TotalPages,
                    successResponse.TotalCount);

                return Ok(successResponse);
            },
            errorResponse =>
            {
                return NotFound(errorResponse.Errors);
            });
    }
}