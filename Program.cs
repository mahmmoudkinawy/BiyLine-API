using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerServices();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddConfigureCors(builder.Configuration);

builder.Services.AddHandfireService(builder.Configuration);


var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer - API Version 1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Trader - API Version 2");
});

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(Constants.Cors.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHangfireDashboard("/dashboard");

app.MapControllers();

app.MapHub<StoreChatHub>("/hubs/storechathub");

await app.ApplyDatabaseMigrations();

await app.RunAsync();
