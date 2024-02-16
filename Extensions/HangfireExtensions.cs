using Hangfire;

namespace BiyLineApi.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHandfireService(this IServiceCollection services , IConfiguration config)
    {
        services.AddHangfire(x => x.UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer(); 

       return services;

    }
}
