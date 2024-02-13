namespace BiyLineApi.Controllers;

[ApiVersion(1.0)]
[ApiVersion(2.0)]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [Authorize(Constants.Policies.ProductRead)]
    public async Task<ActionResult<IReadOnlyList<GetProductsFeature.Response>>> GetProducts(
        [FromQuery] ProductParams productParams)
    {
        var response = await _mediator.Send(new GetProductsFeature.Request
        {
            IsInStock = productParams.IsInStock,
            Predicate = productParams.Predicate,
            PageNumber = productParams.PageNumber,
            PageSize = productParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{productId}")]
    [Authorize(Policy = Constants.Policies.ProductRead)]

    public async Task<ActionResult<GetProductByIdFeature.Response>> GetProductById(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new GetProductByIdFeature.Request
        {
            ProductId = productId
        });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        await _mediator.Send(new SetRecentViewedProductsFeature.Request
        {
            ProductId = productId
        });

        return Ok(response.Value);
    }

    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    [Authorize(Policy = Constants.Policies.ProductRead)]
    [HttpGet("{productId}/details")]
    public async Task<ActionResult<GetProductDetailsByProductIdFeature.Response>> GetProductDetailsById(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new GetProductDetailsByProductIdFeature.Request
        {
            ProductId = productId,
            UserId = User.GetUserById()
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpGet("{productId}/rating-statistics")]
    [Authorize(Policy = Constants.Policies.ProductRead)]

    public async Task<IActionResult> GetRatingStatisticsForProductByProductId(
        [FromRoute] int productId)
    {
        var response = await _mediator
            .Send(new GetRatingStatisticsForProductByProductIdFeature.Request
            {
                ProductId = productId
            });

        return Ok(response);
    }

    [HttpGet("recent-views")]
    [Authorize(Policy = Constants.Policies.ProductRead)]

    public async Task<ActionResult<IReadOnlyList<GetRecentViewedProductsFeature.Response>>> GetRecentViews()
    {
        var response = await _mediator.Send(new GetRecentViewedProductsFeature.Request { });

        return Ok(response);
    }

    [HttpGet("{productId}/rates")]
    [Authorize(Policy = Constants.Policies.ProductRead)]

    public async Task<ActionResult<IReadOnlyList<GetProductRatesByProductIdFeature.Response>>> GetProductRatesByProductId(
        [FromRoute] int productId,
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetProductRatesByProductIdFeature.Request
        {
            ProductId = productId,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{productId}/average-rate")]
    [Authorize(Policy = Constants.Policies.ProductRead)]

    public async Task<ActionResult<GetProductAverageRateByProductIdFeature.Response>> GetAverageProductRatesByProductId(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new GetProductAverageRateByProductIdFeature.Request
        {
            ProductId = productId
        });

        return Ok(response);
    }

    [ApiVersion(2.0)]
    [Authorize(Policy = Constants.Policies.ProductWrite)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromForm] CreateProductFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [ApiVersion(2.0)]
    [Authorize(Policy = Constants.Policies.ProductWrite)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct(
        [FromBody] UpdateProductFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }


    [ApiVersion(2.0)]
    [Authorize(Policy = Constants.Policies.ProductWrite)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] int productId)
    {
        var response = await _mediator.Send(new DeleteProductByTraderFeature.Request
        {
            ProductId = productId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [ApiVersion(2.0)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    [Authorize(Policy = Constants.Policies.ProductWrite)]
    [HttpGet("{productId}/trader")]
    public async Task<ActionResult<GetProductByIdForTraderFeature.Response>> GetProductByIdForTrader([FromRoute] int productId)
    {
        var response = await _mediator.Send(new GetProductByIdForTraderFeature.Request { ProductId = productId });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Value);
    }
}