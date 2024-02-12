namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/basket")]
[Authorize]
public sealed class BasketController : ControllerBase
{
    private readonly IMediator _mediator;

    public BasketController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToBasket(
        [FromBody] AddItemToBasketFeature.Request req)
    {
        var response = await _mediator.Send(req);

        if (!response.IsSuccess)
        {
            return NotFound(response.Error);
        }

        return Ok(response.Value);
    }

    [HttpPost("{productId}/increase-count")]
    public async Task<IActionResult> IncrementBasketItem(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new IncrementBasketItemFeature.Request
        {
            ProductId = productId
        });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{productId}/decrease-count")]
    public async Task<IActionResult> DecrementBasketItem(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new DecrementBasketItemFeature.Request
        {
            ProductId = productId
        });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{productId}/basket-item")]
    public async Task<IActionResult> DeleteBasketItem(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new DeleteBasketItemFeature.Request
        {
            ProductId = productId
        });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("current-user-basket")]
    public async Task<ActionResult<CurrentUserBasketFeature.Response>> GetCurrentUserBasket()
    {
        var response = await _mediator.Send(new CurrentUserBasketFeature.Request { });

        return Ok(response);
    }

    [HttpGet("{addressId}")]
    public async Task<ActionResult<GetCurrentUserBasketWithAddressAndShippingCostFeature.Response>>
        GetAddressAndTotalShippingPrice([FromRoute] int addressId)
    {
        var request =  new GetCurrentUserBasketWithAddressAndShippingCostFeature.Request { AddressId = addressId };
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }
        return Ok(response.Value) ;
    }


}
