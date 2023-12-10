using BiyLineApi.Features.StoreWallet;


namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/storeWallet")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]

public class StoreWalletController : ControllerBase
{

    private readonly IMediator _mediator;
    public StoreWalletController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetStoreWalletsFeature.Response>>> GetStoreWallets(
        [FromQuery] FilterParams filterParams
        )
    {
        var response =  await _mediator.Send(new GetStoreWalletsFeature.Request
        {
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize,
             Predicate = filterParams.Predicate,
        });

        return response.Match<ActionResult<IReadOnlyList<GetStoreWalletsFeature.Response>>>(
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

    [HttpPost]
    public async Task<IActionResult> CreateStoreWallet([FromBody] CreateStoreWalletFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound();
        }
        return Ok();
    }
}

