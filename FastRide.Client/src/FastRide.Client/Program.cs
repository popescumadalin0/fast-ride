using System;
using System.Net.Http;
using FastRide.Client;
using FastRide.Client.Authentication;
using FastRide.Server.Sdk;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSessionStorageServices();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Google", options.ProviderOptions);
});

builder.Services.AddTransient<HttpAuthenticationHandler>();

builder.Services.AddFastRideApiClient<HttpAuthenticationHandler>(new Uri(builder.Configuration["FastRide:BaseUrl"]!));

await builder.Build().RunAsync();