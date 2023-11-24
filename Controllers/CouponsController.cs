namespace BiyLineApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/coupons")]
[Authorize]
public sealed class CouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    [HttpGet("apply/{couponCode}")]
    public async Task<ActionResult<ApplyCouponFeature.Response>> ApplyCouponCode(
        [FromRoute] string couponCode)
    {
        var response = await _mediator.Send(new ApplyCouponFeature.Request
        {
            CouponCode = couponCode
        });

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }


    [ApiVersion(2.0)]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    [EnsureStoreProfileCompleteness]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetCouponsByTraderFeature.Request>>> GetCoupons(
        [FromQuery] CouponParams couponParams)
    {
        var response = await _mediator.Send(new GetCouponsByTraderFeature.Request
        {
            Code = couponParams.Code,
            StartDate = couponParams.StartDate,
            EndDate = couponParams.EndDate,
            PageNumber = couponParams.PageNumber,
            PageSize = couponParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [ApiVersion(2.0)]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    [EnsureStoreProfileCompleteness]
    [HttpPost]
    public async Task<IActionResult> CreateCoupon(
        [FromBody] CreateCouponFeature.Request request)
    {
        await _mediator.Send(request);

        return NoContent();
    }
}
