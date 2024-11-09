using System;
using FastRide.Server.Sdk.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FastRide.Server.Sdk;

/// <summary />
public static class DependencyInjection
{
    /// <summary />
    public static IServiceCollection AddFastRideApiClient(this IServiceCollection services, Uri url)
    {
        services.AddRefitClient<IFastRideApi>()
            .ConfigureHttpClient(c => c.BaseAddress = url);

        services.AddSingleton<IFastRideApiClient, FastRideApiClient>();
        return services;
    }
}