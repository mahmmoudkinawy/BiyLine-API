﻿using BiyLineApi.Attributes;
using BiyLineApi.DbContexts.Migrations;
using BiyLineApi.Features.CashDepostiePermission;
using BiyLineApi.Features.StoreWallet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/cashDepositePermission")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTraderOrEmployee)]
[EnsureSingleStore]
[EnsureStoreProfileCompleteness]

public class CashDepositePermissionController : ControllerBase
{
    private readonly IMediator _mediator;
    public CashDepositePermissionController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost("{storeWalletId}")]   
    public async Task<IActionResult> CreateCashDepositePermission([FromBody] CreateCashDepostiePermissionFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }
        return NoContent();
    }

    [HttpGet("{storeWalletId}")]

    public async Task<ActionResult<IReadOnlyList<GetAllCashDepositePermissionsFeature.Response>>> GetAllCashDepositePermissions(
                [FromQuery] FilterParams filterParams
            )
    {
        var response = await _mediator.Send(new GetAllCashDepositePermissionsFeature.Request
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
