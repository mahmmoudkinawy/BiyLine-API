﻿namespace BiyLineApi.Extensions;
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.ReportApiVersions = true; ;
        })
            .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

        services.AddSwaggerGen(opts =>
        {
            var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

            opts.IncludeXmlComments(xmlCommentsFullPath, true);

            opts.AddSignalRSwaggerGen(_ =>
            {
                _.UseHubXmlCommentsSummaryAsTagDescription = true;
                _.UseHubXmlCommentsSummaryAsTag = true;
                _.UseXmlComments(xmlCommentsFullPath);
            });

            opts.CustomSchemaIds(opts => opts.FullName?.Replace("+", "."));

            opts.AddSecurityDefinition("BiyLineApiBearerAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Input a valid token to access this API"
            });

            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "BiyLineApiBearerAuth"
                        }
                    }, new List<string>() }
            });

            opts.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Customer - API Version 1",
                Description = "Customer endpoints and logic for API Version 1",
                Version = "v1"
            });

            opts.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Trader - API Version 2",
                Description = "Trader endpoints and logic for API Version 2",
                Version = "v2"
            });
            opts.SwaggerDoc("v3", new OpenApiInfo
            {
                Title = "ShippingCompany - API Version 3",
                Description = "Shipping Company endpoints and logic for API Version 3",
                Version = "v3"
            });

            opts.OperationFilter<AcceptLanguageHeaderOperationFilter>();
        });

        return services;
    }
}
