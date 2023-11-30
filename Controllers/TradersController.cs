

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
    public async Task<ActionResult<IReadOnlyList<GetRetailTradersFeature.Response>>> GetTraders(
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
    public async Task<ActionResult<GetTraderById.Response>> GetTraderById(
    [FromRoute] int traderId)
    {
        var response = await _mediator.Send(new GetTraderById.Request
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
}
