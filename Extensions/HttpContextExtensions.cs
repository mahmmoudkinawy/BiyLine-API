namespace BiyLineApi.Extensions;
public static class HttpContextExtensions
{
    public static int GetUserById(this IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor != null ?
            int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
            : throw new ArgumentException(nameof(httpContextAccessor));

    public static int GetValueFromRoute(this IHttpContextAccessor httpContextAccessor, string value) =>
        httpContextAccessor != null ?
            int.Parse(httpContextAccessor.HttpContext.Request.RouteValues[value].ToString())
            : throw new ArgumentException(nameof(httpContextAccessor));
}
