namespace BiyLineApi.Extensions;
public static class CorsExtensions
{
    public static IServiceCollection AddConfigureCors(
        this IServiceCollection services,
        IConfiguration config)
    {
        var originsAllowed = config.GetSection(Constants.Cors.OriginSectionKey)
            .GetChildren()
            .Select(c => c.Value)
            .ToArray();

        if (!originsAllowed.Any()) return services;

        services.AddCors(options =>
        {
            options.AddPolicy(Constants.Cors.PolicyName, policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .WithOrigins(originsAllowed);
            });
        });

        return services;
    }
}
