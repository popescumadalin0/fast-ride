﻿using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Repositories;
using FastRide.Server.Services.Services;
using FastRide.Server.Services.Wrapper;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide.Server.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ITableClient<OnlineDriversEntity>, TableClient<OnlineDriversEntity>>();

        services.AddSingleton<ITableClient<UserEntity>, TableClient<UserEntity>>();
        
        services.AddSingleton<IUserRepository, UserRepository>();
        
        services.AddSingleton<IOnlineDriverRepository, OnlineDriverRepository>();
        
        services.AddSingleton<IUserService, UserService>();
        
        services.AddSingleton<IOnlineDriversService, OnlineDriversService>();

        services.AddSingleton<IDistanceService, DistanceService>();

        return services;
    }
}