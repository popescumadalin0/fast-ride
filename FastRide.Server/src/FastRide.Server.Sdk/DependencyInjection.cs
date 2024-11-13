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
    public static IServiceCollection AddFastRideApiClient<AuthenticationApiHttpMessageHandler>(this IServiceCollection services, Uri url)
        where AuthenticationApiHttpMessageHandler : DelegatingHandler
    {
        services.AddRefitClient<IFastRideApi>()
            .ConfigureHttpClient(c => c.BaseAddress = url)
            .AddHttpMessageHandler<AuthenticationApiHttpMessageHandler>();

        services.AddSingleton<IFastRideApiClient, FastRideApiClient>();
        return services;
    }
}