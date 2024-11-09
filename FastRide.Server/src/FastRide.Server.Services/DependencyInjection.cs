using Azure.Data.Tables;
using FastRide_Server.Services.Contracts;
using FastRide_Server.Services.Entities;
using FastRide_Server.Services.Repositories;
using FastRide_Server.Services.Services;
using FastRide_Server.Services.Wrapper;
using Microsoft.Extensions.DependencyInjection;

namespace FastRide_Server.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ITableClient<UserEntity>, TableClient<UserEntity>>();

        services.AddSingleton<IUserRepository, UserRepository>();

        services.AddSingleton<IUserService, UserService>();

        return services;
    }
}