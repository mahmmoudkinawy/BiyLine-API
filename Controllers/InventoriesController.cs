namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/inventories/{warehouseId}")]
[ApiVersion(2.0)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[ApiController]
public sealed class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventory(
        [FromRoute] int warehouseId,
        [FromBody] CreateInventoryFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("inventory/{inventoryId}")]
    public async Task<IActionResult> UpdateInventory(
        [FromRoute] int warehouseId,
        [FromRoute] int inventoryId,
        [FromBody] UpdateInventoryFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{inventoryId}")]
    public async Task<ActionResult<GetInventoryByIdFeature.Response>> GetInventoryById(
        [FromRoute] int inventoryId)
    {
        var response = await _mediator.Send(new GetInventoryByIdFeature.Request
        {
            InventoryId = inventoryId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response);
    }

    [HttpGet("{inventoryId}/products")]
    public async Task<ActionResult<IReadOnlyList<GetInventoryProductsIdInventoryIdFeature.Response>>> GetInventoryProductsById(
        [FromRoute] int inventoryId,
        [FromQuery] FilterParams filterParams)
    {
        var response = await _mediator.Send(new GetInventoryProductsIdInventoryIdFeature.Request
        {
            PageSize = filterParams.PageSize,
            PageNumber = filterParams.PageNumber,
            Predicate = filterParams.Predicate
        });

        return response.Match<ActionResult<IReadOnlyList<GetInventoryProductsIdInventoryIdFeature.Response>>>(
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

    [HttpDelete("{inventoryId}")]
    public async Task<IActionResult> DeleteInventory(
        [FromRoute] int inventoryId)
    {
        var response = await _mediator.Send(new DeleteInventoryByIdFeature.Request
        {
            InventoryId = inventoryId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
