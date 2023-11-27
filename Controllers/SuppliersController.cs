namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/suppliers")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
public sealed class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetSuppliersFeature.Response>>> GetSuppliers(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetSuppliersFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateOutsideSupplier(
        [FromForm] CreateOutsideSupplierFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPost("{supplierId}")]
    public async Task<IActionResult> CreateInsideSupplier()
    {
        var response = await _mediator.Send(new CreateInsideSupplierFeature.Request { });

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{supplierId}")]
    public async Task<IActionResult> UpdateSupplier(
        [FromForm] UpdateSupplierFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{supplierId}")]
    public async Task<IActionResult> GetSupplierById(
        [FromRoute] int supplierId)
    {
        var response = await _mediator.Send(new GetSupplierByIdFeature.Request
        {
            SupplierId = supplierId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpDelete("{supplierId}")]
    public async Task<IActionResult> DeleteSupplier(
        [FromRoute] int supplierId)
    {
        var response = await _mediator.Send(new DeleteSupplierFeature.Request
        {
            SupplierId = supplierId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{supplierId}/suspend")]
    public async Task<IActionResult> SuspendSupplier(
        [FromRoute] int supplierId)
    {
        var response = await _mediator.Send(new SuspendSupplierFeature.Request
        {
            SupplierId = supplierId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
