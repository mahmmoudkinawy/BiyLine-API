namespace BiyLineApi.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static int GetUserById(this ClaimsPrincipal claims)
        => int.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
