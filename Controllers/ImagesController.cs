namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[ApiController]
[Route("api/v{version:apiVersion}/images")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
public sealed class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ImagesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("{productId}/add-product-image")]
    public async Task<IActionResult> CreateProductImage(
        [FromRoute] int productId,
        [FromForm] AddProductImageByProductIdFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{imageId}/remove-product-image")]
    public async Task<IActionResult> RemoveProductImage(
        [FromRoute] int imageId)
    {
        var response = await _mediator.Send(new RemoveProductImageByProductIdFeature.Request
        {
            ImageId = imageId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
