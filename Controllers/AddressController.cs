using BiyLineApi.Features.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers;


[ApiVersion(2.0)]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/address")]
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
    [Authorize(Constants.Policies.AddressRead)]
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
    [Authorize( Constants.Policies.AddressRead)]


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
    [Authorize(Constants.Policies.AddressWrite)]
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
    [Authorize( Constants.Policies.AddressWrite)]

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
    [Authorize( Constants.Policies.AddressWrite)]

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
