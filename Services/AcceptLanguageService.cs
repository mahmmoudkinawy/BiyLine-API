namespace BiyLineApi.Services;
public sealed class AcceptLanguageService : IAcceptLanguageService
{
    private HttpContext _httpContext;
    private readonly string _language;

    public AcceptLanguageService(
        IHttpContextAccessor contextAccessor)
    {
        _httpContext = contextAccessor?.HttpContext;
        if (_httpContext is not null)
        {
            if (_httpContext.Request.Headers
                    .TryGetValue("Accept-Language", out var language))
            {
                _language = language;
            }
        }
    }

    public string GetLanguageFromHeaderRequest() => _language ?? "ar";
}