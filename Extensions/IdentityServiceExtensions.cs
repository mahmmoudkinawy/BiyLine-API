namespace BiyLineApi.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddIdentityCore<UserEntity>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.SignIn.RequireConfirmedAccount = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
            .AddRoles<RoleEntity>()
            .AddRoleManager<RoleManager<RoleEntity>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddRoleValidator<RoleValidator<RoleEntity>>()
            .AddEntityFrameworkStores<BiyLineDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config[Constants.TokenKey]!)),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "1025888153046-328jrudum7km49euetfirnbnri78e5vj.apps.googleusercontent.com";
                googleOptions.ClientSecret = "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";
            });

        services.AddAuthorization(configure =>
        {
            configure.AddPolicy(Constants.Policies.MustBeTrader, policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, Constants.Roles.Trader);
            });
            configure.AddPolicy(Constants.Policies.MustBeTraderOrEmployee, policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, Constants.Roles.Trader,Constants.Roles.Employee);
            });

        });

        return services;
    }
}
