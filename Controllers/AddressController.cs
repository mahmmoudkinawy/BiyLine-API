using BiyLineApi.Features.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers;


[ApiVersion(2.0)]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/address")]
[Authorize]
[ApiController]
public sealed class AddressController : ControllerBase
{
    private readonly IMediator _mediator;
    public AddressController(IMediator mediator)
    {
        _mediator = mediator ??
                    throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetAddressesForCustomerFeature.Response>>> GetAddresses(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetAddressesForCustomerFeature.Request
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

    [HttpGet("{addressId}")]
    public async Task<ActionResult<GetAddressByIdFeature.Response>> GetAddressById([FromRoute] int addressId)
    {
        var response = await _mediator.Send(new GetAddressByIdFeature.Request { Id = addressId });

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpPut("{addressId}")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] int addressId)
    {
        var response = await _mediator.Send(new DeleteAddressFeature.Request { AddressId = addressId });
        if (!response.IsSuccess)
        {
            return NotFound(response?.Errors);

        }
        return NoContent();
    }

}
