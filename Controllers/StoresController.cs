namespace BiyLineApi.Controllers;

[ApiVersion(2.0)]
[ApiController]
[Route("api/v{version:apiVersion}/stores")]
public sealed class StoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoresController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("ensure-store-profile-complete")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public IActionResult EnsureStoreProfileComplete() => Ok("Now, trader profile is valid");

    [HttpGet("wholesales-traders")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<ActionResult<IReadOnlyList<GetWholesalesFeature.Response>>> GetWholesales(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetWholesalesFeature.Request
        {
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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetStoresFeature.Response>>> GetStores(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetStoresFeature.Request
        {
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

    [HttpPost("details")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateDetailsStore(
       [FromBody] CreateStoreDetailsFeature.Request request)
    {
        await _mediator.Send(request);

        return NoContent();
    }

    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [HttpGet("details")]
    public async Task<ActionResult<GetStoreDetailsFeature.Response>> GetStoreDetails()
    {
        var response = await _mediator.Send(new GetStoreDetailsFeature.Request { });

        return Ok(response);
    }

    [HttpPost("specializations")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateStoreSpecialization(
        [FromForm] CreateStoreSpecializationFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("specializations")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetStoreSpecializationFeature.Response>> GetStoreSpecialization()
    {
        var response = await _mediator.Send(new GetStoreSpecializationFeature.Request { });

        return Ok(response);
    }

    [HttpPost("cover-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateStoreCoverImage(
        [FromForm] CreateStoreCoverFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("cover-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetStoreCoverImageFeature.Response>> GetStoreCoverImage()
    {
        var response = await _mediator.Send(new GetStoreCoverImageFeature.Request { });

        return Ok(response);
    }

    [HttpPost("profile-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateStoreProfileImage(
        [FromForm] CreateStoreProfileImageFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("profile-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetStoreProfileImageFeature.Response>> GetStoreProfileImage()
    {
        var response = await _mediator.Send(new GetStoreProfileImageFeature.Request { });

        return Ok(response);
    }

    [HttpPost("nationalId-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> AddNationalIdImage(
        [FromForm] AddNationalIdImageFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("nationalId-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetNationalIdImageFeature.Response>> GetNationalIdImage()
    {
        var response = await _mediator.Send(new GetNationalIdImageFeature.Request { });

        return Ok(response);
    }

    [HttpPost("tax-document-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> AddTaxDocumentImage(
       [FromForm] AddTaxRegistrationDocumentImageFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("tax-document-image")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetTaxDocumentImageFeature.Response>> GetTaxDocumentImage()
    {
        var response = await _mediator.Send(new GetTaxDocumentImageFeature.Request { });

        return Ok(response);
    }

    [HttpPost("address-with-metadata")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    public async Task<IActionResult> CreateStoreAddressWithMetadata(
       [FromBody] CreateStoreAddressWithMetadataFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("address-with-metadata")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    public async Task<ActionResult<GetStoreAddressWithMetadataFeature.Response>> GetStoreAddressWithMetadata()
    {
        var response = await _mediator.Send(new GetStoreAddressWithMetadataFeature.Request { });

        return Ok(response);
    }

    [HttpGet("employees-salary-calculation")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureSingleStore]
    [EnsureStoreProfileCompleteness]
    public async Task<ActionResult<GetStoreEmployeesSalaryCalculationFeature.Response>> GetStoreEmployeesSalaryCalculation()
    {
        var response = await _mediator.Send(new GetStoreEmployeesSalaryCalculationFeature.Request { });

        return Ok(response);
    }

    [HttpGet("current-store-products")]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [EnsureStoreProfileCompleteness]
    [EnsureSingleStore]
    public async Task<ActionResult<IReadOnlyList<GetCurrentStoreProductsFeature.Response>>> GetCurrentStoreProducts(
       [FromQuery] StoreProductsParams storeProductsParams)
    {
        var response = await _mediator.Send(new GetCurrentStoreProductsFeature.Request
        {
            Status = storeProductsParams.Status,
            IsInStock = storeProductsParams.IsInStock,
            CodeNumber = storeProductsParams.CodeNumber,
            Name = storeProductsParams.Name,
            PageNumber = storeProductsParams.PageNumber,
            PageSize = storeProductsParams.PageSize
        });

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{storeId}/products")]
    public async Task<ActionResult<IReadOnlyList<GetProductsByStoreIdFeature.Response>>> GetProductsByStoreId(
        [FromRoute] int storeId,
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetProductsByStoreIdFeature.Request
        {
            StoreId = storeId,
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
}
