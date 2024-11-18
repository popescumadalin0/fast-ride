using FastRide.Server.Sdk;

var builder = WebApplication.CreateBuilder(args);

ConfigAzureAppConfig();
await ConfigureServicesAsync();
ConfigureCors();
ConfigureAuthentication();

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.UseSwaggerUI();

await app.RunAsync();

return;

async Task ConfigureServicesAsync()
{
    builder.Services.AddLogging();
    builder.Services.AddControllers();
    
    builder.Services.AddFastRideApiClient(new Uri(builder.Configuration["FastRideApi:BaseUrl"]!));

    builder.Services.AddRazorPages();
}

void ConfigAzureAppConfig()
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
}

void ConfigureCors()
{
    builder.Services.AddCors(cors => cors
        .AddDefaultPolicy(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));
}

void ConfigureAuthentication()
{
    
}