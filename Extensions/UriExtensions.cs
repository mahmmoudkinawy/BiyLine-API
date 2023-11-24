namespace BiyLineApi.Extensions;
public static class UriExtensions
{
    public static string CombineUri(this Uri value, string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
        {
            return null;
        }

        if (Uri.TryCreate(value, relativeUrl, out var combinedUri))
        {
            return combinedUri.AbsoluteUri;
        }

        return relativeUrl;
    }

    public static Uri BaseUri(this IHttpContextAccessor value, string name) =>
        value != null ? new Uri(value.HttpContext.Request.Scheme + "://" + value.HttpContext.Request.Host)
        : throw new ArgumentException(nameof(name));
}
