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

    public async Task<ServiceResponse<UserTypeResponse>> GetUserType(UserIdentifier user)
    {
        try
        {
            var userType = await _userRepository.GetUserTypeAsync(user.NameIdentifier, user.Email);

            if (userType == null)
            {
                var registerUser = await _userRepository.AddOrUpdateUserAsync(new UserEntity()
                {
                    UserType = UserType.User,
                    PartitionKey = user.Email,
                    RowKey = user.NameIdentifier,
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

    public async Task<ServiceResponse> UpdateUserRating(UserRating userRating)
    {
        try
        {
            var actualUser = await _userRepository.GetUserAsync(userRating.User.NameIdentifier, userRating.User.Email);

            var response = await _userRepository.AddOrUpdateUserAsync(new UserEntity()
            {
                PartitionKey = userRating.User.Email,
                RowKey = userRating.User.NameIdentifier,
                UserType = actualUser.UserType,
                Rating = CalculateRating(actualUser.Rating, userRating.Rating)
            });

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }
            
            return new ServiceResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse(ex);
        }
    }

    private static double CalculateRating(double currentRating, int rating)
    {
        return (currentRating + rating) / 2;
    }
}