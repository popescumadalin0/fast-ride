using System;
using FastRide.Server.Attributes;
using FastRide.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<AuthenticationMiddleware>();
        //builder.UseMiddleware<AuthorizationMiddleware>();
    })
    .ConfigureAppConfiguration((context, builder) => { builder.AddUserSecrets<Program>(); })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("Google:ClientId")!;
                options.ClientSecret = Environment.GetEnvironmentVariable("Google:ClientSecret")!;
                options.SaveTokens = true;
            });

        services.AddLogging();

        services.AddServices();
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