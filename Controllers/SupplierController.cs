using AutoMapper.Configuration.Conventions;
using BiyLineApi.DbContexts.Migrations;
using BiyLineApi.Features.Supplier;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BiyLineApi.Controllers;

[Route("api/v{version:apiVersion}/suppliers")]
[ApiController]
[ApiVersion("2.0")]
[Authorize(Policy = Constants.Policies.MustBeTrader)] 
[EnsureSingleStore]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator ??
        throw new ArgumentNullException(nameof(mediator));
    }


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetSuppliersFeature.Response>>> GetSuppliers([FromQuery] PaginationParams paginationParams)
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
    public async Task<IActionResult> AddOutsideSupplier([FromForm] AddOutsideSupplierFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);

        }
        return NoContent();
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> AddInsideSupplier()
    {
        var response = await _mediator.Send(new AddInsideSupplierFeature.Request { });

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);

        }
        return NoContent();
    }


   
    [HttpPut("{supplierId}")]

    public async Task<IActionResult> UpdateSupplier([FromForm] UpdateSupplierFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

   
    [HttpGet("{supplierId}")]

    public async Task<IActionResult> GetSupplierById()
    {
       
        var response = await _mediator.Send(new GetSupplierById.Request());

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value); 
    }

   
    [HttpDelete("{supplierId}")]

    public async Task<IActionResult> DeleteSupplier()
    {
        var response = await _mediator.Send(new DeleteSupplierFeature.Request());

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

   
    [HttpPut("{supplierId}/suspend")]
    public async Task<IActionResult> SuspendSupplier()
    {
        var response = await _mediator.Send(new SuspendSupplierFeature.Request());

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }
}
