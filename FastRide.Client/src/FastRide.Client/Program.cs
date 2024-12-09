using System;
using System.Net.Http;
using FastRide.Client;
using FastRide.Client.Authentication;
using FastRide.Client.Contracts;
using FastRide.Client.Models;
using FastRide.Client.Service;
using FastRide.Client.State;
using FastRide.Server.Sdk;
using GoogleMapsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<DestinationState>();

builder.Services.AddScoped<OverlayState>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSessionStorageServices();

builder.Services.AddTransient<HttpAuthenticationHandler>();

builder.Services.AddFastRideApiClient<HttpAuthenticationHandler>(new Uri(builder.Configuration["FastRide:BaseUrl"]!));

builder.Services.AddOidcAuthentication<RemoteAuthenticationState,
        CustomUserAccount>(options => { builder.Configuration.Bind("Google", options.ProviderOptions); })
    .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, CustomUserAccount, CustomUserFactory>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IGeolocationService, GeolocationService>();

builder.Services.AddScoped<IDistanceService, DistanceService>();

var navigation = builder.Services.BuildServiceProvider().GetRequiredService<NavigationManager>();

var hubConnection = new HubConnectionBuilder()
    .WithUrl(navigation.ToAbsoluteUri("/ride"))
    .Build();

builder.Services.AddSingleton(hubConnection);

builder.Services.AddScoped<ISignalRObserver, SignalRObserver>();

builder.Services.AddScoped<ISignalRSender, SignalRSender>();

builder.Services.AddBlazorGoogleMaps(builder.Configuration["GoogleMaps:ApiKey"]!);

builder.Services.AddMudServices();

await builder.Build().RunAsync();