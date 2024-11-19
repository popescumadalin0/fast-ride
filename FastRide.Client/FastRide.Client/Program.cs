using System;
using System.Net.Http;
using Blazored.LocalStorage;
using FastRide.Client;
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

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Google", options.ProviderOptions);
    //options.ProviderOptions.DefaultScopes.Add("https://localhost:7062");
});

builder.Services.AddFastRideApiClient(new Uri(builder.Configuration["FastRide:BaseUrl"]!));

await builder.Build().RunAsync();