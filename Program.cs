var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerServices();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddConfigureCors(builder.Configuration);

var app = builder.Build();

//app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer - API Version 1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Trader - API Version 2");
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "ShippingCompany - API Version 3");
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(Constants.Cors.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllers();

app.MapHub<StoreChatHub>("/hubs/storechathub");

await app.ApplyDatabaseMigrations();

await app.RunAsync();


////public class ExceptionMiddleware
//{
//    private readonly RequestDelegate _next;
//    private readonly IWebHostEnvironment _env;
//    private readonly ILogger<ExceptionMiddleware> _logger;

//    public ExceptionMiddleware(
//        RequestDelegate next,
//        IWebHostEnvironment env,
//        ILogger<ExceptionMiddleware> logger)
//    {
//        _next = next;
//        _env = env;
//        _logger = logger;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        try
//        {
//            await _next(context);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, ex.Message);
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

//            var response = _env.IsDevelopment() ?
//                new ProblemDetails
//                {
//                    Status = context.Response.StatusCode,
//                    Instance = ex.Message,
//                    Detail = ex.StackTrace
//                }
//                :
//                new ProblemDetails
//                {
//                    Status = context.Response.StatusCode,
//                    Instance = "Internal Server Error"
//                };

//            var options = new JsonSerializerOptions
//            {
//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//            };

//            var json = JsonSerializer.Serialize(response, options);

//            await context.Response.WriteAsync(json);
//        }
//    }
//}