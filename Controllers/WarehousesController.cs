namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/warehouses")]
[ApiVersion(2.0)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[ApiController]
public sealed class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetWarehousesForStoreFeature.Response>>> GetWarehousesForStore(
        [FromQuery] FilterParams filterParams)
    {
        var response = await _mediator.Send(new GetWarehousesForStoreFeature.Request
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

    [HttpGet("{warehouseId}/products")]
    public async Task<ActionResult<IReadOnlyList<GetWarehouseProductByWarehouseIdFeature.Response>>>
        GetWarehousesProductsForStore(
            [FromRoute] int warehouseId,
            [FromQuery] WarehousesProductParams warehousesProductParams)
    {
        var response = await _mediator.Send(new GetWarehouseProductByWarehouseIdFeature.Request
        {
            WarehouseId = warehouseId,
            CodeNumber = warehousesProductParams.CodeNumber,
            Name = warehousesProductParams.Name,
            IsInStockStatus = warehousesProductParams.Status,
            OrderBy = warehousesProductParams.OrderBy,
            PageNumber = warehousesProductParams.PageNumber,
            PageSize = warehousesProductParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetWarehouseProductByWarehouseIdFeature.Response>>>(
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

    [HttpGet("{warehouseId}/statistics")]
    public async Task<ActionResult<GetWarehousesStatisticsFeature.Response>> GetWarehousesStatisticsForStore(
        [FromRoute] int warehouseId)
    {
        var response = await _mediator.Send(new GetWarehousesStatisticsFeature.Request
        {
            WarehouseId = warehouseId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouseForStore(
        [FromBody] CreateWarehouseForStoreFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{warehouseId}")]
    public async Task<IActionResult> UpdateWarehouseForStore(
        [FromRoute] int warehouseId,
        [FromBody] UpdateWarehouseForStoreFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{warehouseId}")]
    public async Task<IActionResult> DeleteWarehouseForStore(
        [FromRoute] int warehouseId)
    {
        var response = await _mediator.Send(new DeleteWarehouseForStoreFeature.Request
        {
            WarehouseId = warehouseId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
