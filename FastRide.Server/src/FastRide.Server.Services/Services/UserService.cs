using System;
using System.Diagnostics.SymbolStore;
using System.Threading.Tasks;
using FastRide.Server.Contracts;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Models;
using Microsoft.Extensions.Logging;
using UserType = FastRide.Server.Services.Enums.UserType;

namespace FastRide.Server.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserTypeResponse>> GetUserType(string nameIdentifier, string email)
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

                if (registerUser.IsError)
                {
                    throw new Exception(registerUser.ReasonPhrase);
                }

                userType = UserType.User;
            }

            return new ServiceResponse<UserTypeResponse>(new UserTypeResponse
                { UserType = (Server.Contracts.UserType)userType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<UserTypeResponse>(ex);
        }
    }
}