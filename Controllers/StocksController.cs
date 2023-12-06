namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/stocks")]
[ApiVersion(2.0)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[ApiController]
public sealed class StocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public StocksController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetStocksMovementsFeature.Response>>> GetStocks(
        [FromQuery] StockParams stockParams)
    {
        var response = await _mediator.Send(new GetStocksMovementsFeature.Request
        {
            Date = stockParams.Date,
            DestinationWarehouseId = stockParams.DestinationWarehouseId,
            InvoiceNumber = stockParams.InvoiceNumber,
            PageNumber = stockParams.PageNumber,
            PageSize = stockParams.PageSize,
            SourceWarehouseId = stockParams.SourceWarehouseId
        });

        return response.Match<ActionResult<IReadOnlyList<GetStocksMovementsFeature.Response>>>(
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

    [HttpGet("{stockId}")]
    public async Task<ActionResult<GetStockMovementByStockIdFeature.Response>> GetStockById(
        [FromRoute] int stockId)
    {
        var response = await _mediator.Send(new GetStockMovementByStockIdFeature.Request
        {
            StockId = stockId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStock(
        [FromBody] CreateStockMovementFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStock(
        [FromRoute] int stockId)
    {
        var response = await _mediator.Send(new DeleteStockMovementByStockIdFeature.Request
        {
            StockId = stockId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
