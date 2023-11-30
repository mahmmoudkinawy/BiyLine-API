

namespace BiyLineApi.Controllers
{
    [Route("api/v{version:apiVersion}/traders")]
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public class TraderController : ControllerBase
    {

        private readonly IMediator _mediator;

        public TraderController(IMediator mediator)
        {

            _mediator = mediator ??
                        throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GetAllTradersExceptSectionalFeature.Response>>> GetTraders(
        [FromQuery] TraderParams paginationParams)
        {
            var response = await _mediator.Send(new GetAllTradersExceptSectionalFeature.Request
            {
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                Predicate = paginationParams.Predicate
            });

            Response.AddPaginationHeader(
                response.CurrentPage,
                response.PageSize,
                response.TotalPages,
                response.TotalCount);

            return Ok(response);
        }

        [HttpGet("{traderId}")]

        public async Task<IActionResult> GetTraderById(
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
}
