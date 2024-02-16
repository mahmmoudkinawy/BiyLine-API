using BiyLineApi.Features.Shipment.Commands.CreateShipment;
using BiyLineApi.Features.Shipment.Commands.UpdateShipmentShipping;
using BiyLineApi.Features.Shipment.Commands.UpdateShipmentStatus;
using BiyLineApi.Features.Shipment.Queries.GetShipment;
using BiyLineApi.Features.Shipment.Queries.GetShipments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiyLineApi.Controllers
{
    [ApiVersion(3.0)]
    [ApiVersion(2.0)]
    [ApiVersion(1.0)]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ShipmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShipmentController(IMediator mediator)
        {
            _mediator = mediator ??
                throw new ArgumentNullException(nameof(mediator));
        }
        [HttpGet("GetShipment")]
        public async Task<ActionResult<GetShipmentQuery.Response>>
        GetShipment(int Id)
        {
            var request = new GetShipmentQuery.Request {Id = Id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
        [HttpGet("GetShipments")]
        public async Task<ActionResult<GetShipmentsQuery.Response>>
        GetShipments()
        {
            var request = new GetShipmentsQuery.Request {};
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
        
        
        [HttpPost("CreateShipment")]
        public async Task<ActionResult<CreateShipmentCommand.Response>>
        CreateShipment([FromBody] CreateShipmentCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
        
        [HttpPost("UpdateShipmentShipping")]
        public async Task<ActionResult<UpdateShipmentShippingCommand.Response>>
        UpdateShipmentShipping([FromBody] UpdateShipmentShippingCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
        [HttpPost("UpdateShipmentShipping")]
        public async Task<ActionResult<UpdateShipmentStatusCommand.Response>>
        UpdateShipmentStatus([FromBody] UpdateShipmentStatusCommand.Request request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }
    }
}
