namespace BiyLineApi.Controllers;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/metadata")]
[ApiController]
public sealed class MetadataController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetadataController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("valid-types")]
    public IActionResult GetValidDocumentTypes() => Ok(Enum.GetNames(typeof(DocumentTypeEnum)));

    [HttpGet]
    public async Task<ActionResult<GetDocumentByTypeFeature.Response>> GetDocumentByType(
        [FromQuery] string? documentType)
    {
        var response = await _mediator.Send(new GetDocumentByTypeFeature.Request
        {
            DocumentType = documentType
        });

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Value);
    }

}
