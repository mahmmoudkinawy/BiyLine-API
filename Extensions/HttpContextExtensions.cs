using Microsoft.AspNetCore.Http;

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

    public static string GetUserRole(this IHttpContextAccessor httpContextAccessor)=>
        httpContextAccessor!= null ?
        httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)
        : throw new ArgumentException(nameof(httpContextAccessor));
}
