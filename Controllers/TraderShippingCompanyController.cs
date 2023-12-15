namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/traderShippingCompany")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTrader)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]
public class TraderShippingCompanyController : ControllerBase
{
    private readonly IMediator _mediator;
    public TraderShippingCompanyController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTraderShippingCompany(
        [FromBody] CreateTraderShippingCompanyFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetTraderShippingCompaniesFeature.Response>>> GetTraderShippingCompanies([FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetTraderShippingCompaniesFeature.Request
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

    [HttpGet("{traderShippingCompanyId}")]
    public async Task<ActionResult<GetTraderShippingCompanyByIdFeature.Response>> GetTraderShippingCompanyById([FromRoute] int traderShippingCompanyId)
    {
        var response = await _mediator.Send(new GetTraderShippingCompanyByIdFeature.Request
        {
            TraderShippingCompanyId = traderShippingCompanyId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPut("{traderShippingCompanyId}")]
    public async Task<IActionResult> UpdateTraderShippingCompany(
        [FromBody] UpdateTraderShippingCompanyFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPost("{traderShippingCompanyId}/governorate/{governorateId}")]
    public async Task<IActionResult> CreateGovernorate(
        [FromBody] CreateGovernorateFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{traderShippingCompanyId}/governorates")]
    public async Task<ActionResult<IReadOnlyList<GetAllGovernoratesForTraderShippingCompanyFeature.Response>>> GetAllGovernoratesForTraderShippingCompany([FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetAllGovernoratesForTraderShippingCompanyFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetAllGovernoratesForTraderShippingCompanyFeature.Response>>>(
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

    [HttpGet("governorates/{shippingGovernorateId}")]
    public async Task<ActionResult<GetShippingGovernorateByIdFeature.Response>> GetShippingGovernorateById([FromRoute] int shippingGovernorateId)
    {
        var response = await _mediator.Send(new GetShippingGovernorateByIdFeature.Request
        {
            ShippingGovernorateId = shippingGovernorateId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }

    [HttpPut("governorates/{shippingGovernorateId}")]
    public async Task<IActionResult> UpdateShippingGovernorator(
        [FromBody] UpdateShippingGovernoratorFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPost("shippingGovernorate/{shippingGovernorateId}/shippingCenter")]
    public async Task<IActionResult> CreateShippingCenterForShippingGovernorate(
        [FromBody] CreateShippingCenterFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("shippingGovernorate/{shippingGovernorateId}/shippingCenters")]
    public async Task<ActionResult<IReadOnlyList<GetAllShippingCentersForShippingGovernorateFeature.Response>>> GetAllShippingCentersForShippingGovernorate(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetAllShippingCentersForShippingGovernorateFeature.Request
        {
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        });

        return response.Match<ActionResult<IReadOnlyList<GetAllShippingCentersForShippingGovernorateFeature.Response>>>(
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

    [HttpGet("shippingGovernorate/shippingCenters/{shippingCenterId}")]
    public async Task<ActionResult<GetShippingCenterByIdFeature.Response>> GetShippingCenterById(
        [FromRoute] int shippingCenterId)
    {
        var response = await _mediator.Send(new GetShippingCenterByIdFeature.Request
        {
            ShippingCenterId = shippingCenterId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }
        return Ok(response.Value);
    }

    [HttpPut("shippingGovernorate/shippingCenters/{shippingCenterId}")]
    public async Task<IActionResult> UpdateShippingCenter([FromBody] UpdateShippingCenterFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("shippingGovernorate/shippingCenters/{shippingCenterId}/status")]
    public async Task<IActionResult> UpdateShippingCenterStatus([FromBody] UpdateShippingCenterStatusFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if (response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("shippingGovernorates/{shippingGovernorateId}/shippingCenters")]
    public async Task<IActionResult> ApplyTheGovernoratePropertiesOnAllCenters([FromRoute] int shippingGovernorateId)
    {
        var response = await _mediator.Send(new ApplyTheGovernoratePropertiesOnAllCentersFeature.Request
        {
            ShippingGovernorateId = shippingGovernorateId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("shippingGovernorate/shippingCenters/{shippingCenterId}")]
    public async Task<IActionResult> DeleteShippingCenter([FromRoute] int shippingCenterId)
    {
        var response = await _mediator.Send(new DeleteShippingCenterFeature.Request
        {
            ShippingCenterId = shippingCenterId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("shippingGovernorate/{shippingGovernorateId}")]
    public async Task<IActionResult> DeleteShippingGovernorate([FromRoute] int shippingGovernorateId)
    {
        var response = await _mediator.Send(new DeleteShippingGovernorateFeature.Request
        {
            ShippingGovernorateId = shippingGovernorateId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{traderShippingCompanyId}")]
    public async Task<IActionResult> DeleteTraderShippingCompany([FromRoute] int traderShippingCompanyId)
    {
        var response = await _mediator.Send(new DeleteTraderShippingCompanyFeature.Request
        {
            TraderShippingCompanyId = traderShippingCompanyId
        });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
