using System;
using System.Threading.Tasks;
using Azure;
using FastRide_Server.Services.Contracts;
using FastRide_Server.Services.Entities;
using FastRide_Server.Services.Enums;
using FastRide_Server.Services.Models;
using FastRide_Server.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace FastRide_Server.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserType>> GetUserType(string nameIdentifier, string email)
    {
        try
        {
            var userType = await _userRepository.GetUserType(nameIdentifier, email);

            if (userType == null)
            {
                var registerUser = await _userRepository.RegisterUser(new UserEntity()
                {
                    UserType = UserType.User,
                    PartitionKey = nameIdentifier,
                    RowKey = email,
                });

                if (!registerUser.IsError)
                {
                    throw new Exception(registerUser.ReasonPhrase);
                }

                userType = UserType.User;
            }

            return new ServiceResponse<UserType>((UserType)userType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<UserType>(ex);
        }
    }
}