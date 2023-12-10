using BiyLineApi.Features.CashDepostiePermission;
using BiyLineApi.Features.CashDiscountPermission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/cashDiscountPermission")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTraderOrEmployee)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]

public class CashDiscountPermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    public CashDiscountPermissionController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("{storeWalletId}")]
    public async Task<IActionResult> CreateCashDiscountPermission([FromBody]  CreateCashDiscountPermissionFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        if(response.IsBadRequest)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

    [HttpGet("{storeWalletId}")]

    public async Task<ActionResult<IReadOnlyList<GetAllCashDiscountPermissionsFeature.Response>>> GetAllCashDepositePermissions(
                [FromQuery] FilterParams filterParams
            )
    {
        var response = await _mediator.Send(new GetAllCashDiscountPermissionsFeature.Request
        {
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize,
            Predicate = filterParams.Predicate,
        });

        Response.AddPaginationHeader(
           response.CurrentPage,
           response.PageSize,
           response.TotalPages,
           response.TotalCount);

        return Ok(response);
    }
}
