using BiyLineApi.Features.Products.Queries;

namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/stockTrackers/{warehouseId}")]
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

    [HttpPost]
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

    [HttpGet]
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

    [HttpGet("check-threshold")]
    public async Task<ActionResult<IReadOnlyList<GetThresholdedProductsFeature.Response>>> CheckThreshold(
        [FromRoute] int warehouseId,
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetThresholdedProductsFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetThresholdedProductsFeature.Response>>>(
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

    [HttpPost("{productId}/supplier-order")]
    public async Task<IActionResult> CreateSupplierOrder(
        [FromRoute] int productId,
        [FromRoute] int warehouseId,
        [FromBody] CreateSupplierOrderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}