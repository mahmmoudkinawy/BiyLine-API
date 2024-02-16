using BiyLineApi.Features.Shipment.Commands.CreateShipment;
using BiyLineApi.Features.Shipment.Queries.GetShipments;
using BiyLineApi.Features.Warehouses.WareHouseLog.Commands.CreateWarehouse;
using BiyLineApi.Features.Warehouses.WareHouseLog.Queries.GetWarehouseLog;
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
    public class WarehouseLogController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WarehouseLogController(IMediator mediator)
        {
            _mediator = mediator ??
                throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("GetWarehouseLog")]
        public async Task<ActionResult<GetWarehouseLogs.Response>>
        GetWarehouseLog(int DocId , DocumentType documentType)
        {
            var request = new GetWarehouseLogs.Request {DocumentId = DocId , DocumentType = documentType };
            var response = await _mediator.Send(request);

            if (!response.IsSuccess)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }


        [HttpPost("CreateWarehouseLog")]
        public async Task<ActionResult<CreateWarehouseLog.Response>>
        CreateWarehouseLog([FromBody] CreateWarehouseLog.Request request)
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
