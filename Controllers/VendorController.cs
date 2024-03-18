using BiyLineApi.Features.Vendor.Commands.CreateVendor;
using BiyLineApi.Features.Vendor.Commands.DeleteVendor;
using BiyLineApi.Features.Vendor.Commands.UpdateVendor;
using BiyLineApi.Features.Vendor.Queries.GetVendor;
using BiyLineApi.Features.Vendor.Queries.GetVendors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers
{

    [Route("api/v{version:apiVersion}/vendor")]
    [ApiVersion(2.0)]
    [EnsureSingleStore]
    [EnsureStoreProfileCompleteness]
    [Authorize(Policy = Constants.Policies.MustBeTrader)]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendorController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpGet("GetVendors")]
        public async Task<ActionResult<IReadOnlyList<GetVendorsQuery.Response>>> GetVendors(
       [FromQuery] FilterParams filterParams)
        {
            var response = await _mediator.Send(new GetVendorsQuery.Request
            {
                Predicate = filterParams.Predicate,
                PageNumber = filterParams.PageNumber,
                PageSize = filterParams.PageSize
            });

            Response.AddPaginationHeader(
                response.CurrentPage,
                response.PageSize,
                response.TotalPages,
                response.TotalCount);

            return Ok(response);
        }

        [HttpGet("GetVendor")]
        public async Task<ActionResult<GetVendorQuery.Response>> GetVendor(
       [FromRoute] int Id)
        {
            var response = await _mediator.Send(new GetVendorQuery.Request
            {
                Id = Id
            });

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }

            return Ok(response.Value);

        }
        [HttpPost("CreateVendor")]
        public async Task<IActionResult> CreateVendor(
       [FromBody] CreateVendorCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Value);
        }
        
        [HttpPost("UpdateVendor")]
        public async Task<IActionResult> UpdateVendor(
       [FromBody] UpdateVendorCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Value);
        }
        
        [HttpDelete("DeleteVendor")]
        public async Task<IActionResult> DeleteVendor(
       [FromBody] DeleteVendorCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Value);
        }

       
    }
}
