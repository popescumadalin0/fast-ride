using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddUserSecrets<Program>();
        var builtConfig = builder.Build();
        /*builder.BuildFlanderijnAppConfiguration(o =>
        {
            o.AzureConfigEndpoint = builtConfig["AppConfigEndpoint"];
            o.AzureConfigConnectionString = builtConfig["AppConfigConnection"];
            o.Filter.Select("*", "AppTemplates");
        });*/
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        //services.AddDeploymentMetadataEndpoint(context.Configuration["AppConfigEndpoint"]);
    })
    .ConfigureLogging((context, b) =>
    {
        //https://github.com/Azure/azure-functions-host/issues/8973#issuecomment-1890784686
        b.AddConsole();
        b.AddFilter("", LogLevel.Information);
        b.AddFilter("Azure.Core", LogLevel.Warning);
    })
    .Build();

host.Run();