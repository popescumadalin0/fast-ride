using System;
using System.Net.Http;
using FastRide.Server.Sdk.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FastRide.Server.Sdk;

/// <summary />
public static class DependencyInjection
{
    /// <summary />
    public static IServiceCollection AddFastRideApiClient<THttpDelegatingHandler>(this IServiceCollection services,
        Uri url)
        where THttpDelegatingHandler : DelegatingHandler
    {
        services.AddRefitClient<IFastRideApi>()
            .ConfigureHttpClient(c => c.BaseAddress = url)
            .AddHttpMessageHandler<THttpDelegatingHandler>();

        services.AddSingleton<IFastRideApiClient, FastRideApiClient>();
        return services;
    }
}