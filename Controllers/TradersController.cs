namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/traders")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
public class TradersController : ControllerBase
{

    private readonly IMediator _mediator;

    public TradersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetRetailTradersFeature.Response>>> GetRetailTraders(
    [FromQuery] TraderParams traderParams)
    {
        var response = await _mediator.Send(new GetRetailTradersFeature.Request
        {
            PageNumber = traderParams.PageNumber,
            PageSize = traderParams.PageSize,
            Predicate = traderParams.Predicate
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{traderId}")]
    public async Task<ActionResult<GetRetailTraderById.Response>> GetRetailTraderById(
    [FromRoute] int traderId)
    {
        var response = await _mediator.Send(new GetRetailTraderById.Request
        {
            Id = traderId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPost("{supplierId}/contract-order")]
    public async Task<IActionResult> CreateContractOrder([FromBody] CreateContractOrderFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("contractOrders/{contractOrderId}")]
    public async Task<IActionResult> CancelContractOrder([FromRoute] int contractOrderId)
    {
        var response = await _mediator.Send(new CancelContractOrderFeature.Request { ContractOrderId = contractOrderId });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpPut("contractOrders/{contractOrderId}/delivered-status")]
    public async Task<IActionResult> UpdateContractOrderStatusToRejected([FromRoute] int contractOrderId)
    {
        var response = await _mediator.Send(new UpdateContractOrderStatusToDeliverdFeature.Request { ContractOrderId = contractOrderId });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("supplierInvoice/{supplierInvoiceId}")]
    public async Task<IActionResult> UpdateSupplierInvoice([FromBody] UpdateSupplierInvoiceFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Error);
        }

        return NoContent();
    }
}
