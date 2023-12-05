using BiyLineApi.Features.Contractor;
using BiyLineApi.Features.ContractOrder;
using BiyLineApi.Features.SupplierInvoice;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    [HttpGet("retailSuppliers")]
    public async Task<ActionResult<IReadOnlyList<GetRetailSuppliersFeature.Response>>> GetRetailSuppliers(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetRetailSuppliersFeature.Request
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


    [HttpGet("contractOrders")]
    public async Task<ActionResult<IReadOnlyList<GetAllContractOrdersFeature.Response>>> GetContractOrders(
        [FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetAllContractOrdersFeature.Request
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

    [HttpGet("contractOrders/{contractOrderId}")]
    public async Task<ActionResult<GetContractOrderByIdFeature.Response>> GetContractOrderById([FromRoute] int contractOrderId)
    {
        var response = await _mediator.Send(new GetContractOrderByIdFeature.Request { ContractOrderId = contractOrderId });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Value);
    }


    [HttpPut("contractOrders/{contractOrderId}/shipping-status")]

    public async Task<IActionResult> UpdateContractOrderStatusToShipping([FromBody] CreateSupplierInvoiceFeature.Request request)
    {
        var updatedStatusResponse = await _mediator.Send(new UpdateContractOrderStateToShippingFeature.Request { });

        if (!updatedStatusResponse.IsSuccess)
        {
            return NotFound();
        }

        var createdSupplierInvoiceResponse = await _mediator.Send(new CreateSupplierInvoiceFeature.Request { PaidAmount = request.PaidAmount, ShippingPrice = request.ShippingPrice });

        if (!createdSupplierInvoiceResponse.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("contractOrders/{contractOrderId}/rejected-status")]

    public async Task<IActionResult> UpdateContractOrderStatusToRejected([FromBody] UpdateContractOrderStateToRejectedFeature.Request request)
    {
        var response = await _mediator.Send(request);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("contractors")]

    public async Task<ActionResult<IReadOnlyList<GetAllContractorsFeature.Response>>> GetAllContractors([FromQuery] PaginationParams paginationParams)
    {
        var response = await _mediator.Send(new GetAllContractorsFeature.Request
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

    
}

