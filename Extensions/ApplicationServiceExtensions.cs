namespace BiyLineApi.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<BiyLineDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddControllers()
             .AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<Program>());

        services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
            {
                new("en"),
                new("ar")
            };

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        services.AddMediatR(_ => _.RegisterServicesFromAssemblyContaining<Program>());

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddHttpContextAccessor();

        services.AddScoped<IAcceptLanguageService, AcceptLanguageService>();

        services.AddTransient<ITokenService, TokenService>();

        services.AddTransient<IMailService, MailService>();

        services.AddTransient<IImageService, ImageService>();

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    public static async Task ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BiyLineDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
        try
        {
            //await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();
            await Seed.SeedRolesAsync(roleManager);
            await Seed.SeedUsersAsync(userManager);
            await Seed.SeedCategoriesWithRelatedDataAsync(dbContext);
            await Seed.SeedCountriesWithGovernoratesAndRegionsAsync(dbContext);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while applying pending migrations.");
        }
    }

}
