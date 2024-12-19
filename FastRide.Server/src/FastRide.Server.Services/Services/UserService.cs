using System;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
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

    public async Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user)
    {
        try
        {
            var actualUser = await _userRepository.GetUserAsync(user.NameIdentifier, user.Email);

            if (actualUser == null)
            {
                actualUser = new UserEntity()
                {
                    UserType = UserType.User,
                    PartitionKey = user.Email,
                    RowKey = user.NameIdentifier,
                };
                var registerUser = await _userRepository.AddOrUpdateUserAsync(actualUser);

                if (registerUser.IsError)
                {
                    throw new Exception(registerUser.ReasonPhrase);
                }
            }

            return new ServiceResponse<User>(new User()
            {
                UserType = (Server.Contracts.Enums.UserType)actualUser.UserType,
                Identifier = new UserIdentifier()
                {
                    NameIdentifier = actualUser.RowKey,
                    Email = actualUser.PartitionKey,
                },
                Rating = actualUser.Rating,
                PhoneNumber = actualUser.PhoneNumber,
                PictureUrl = actualUser.PictureUrl,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<User>(ex);
        }
    }

    public async Task<ServiceResponse> UpdateUserAsync(UserIdentifier user, UpdateUserPayload updateUserPayload,
        string pictureUrl)
    {
        try
        {
            var actualUser = await _userRepository.GetUserAsync(user.NameIdentifier, user.Email);

            if (actualUser == null)
            {
                return new ServiceResponse(errorMessage: "User not found");
            }

            var response = await _userRepository.AddOrUpdateUserAsync(new UserEntity()
            {
                PartitionKey = actualUser.PartitionKey,
                RowKey = actualUser.RowKey,
                UserType = actualUser.UserType,
                Rating = actualUser.Rating,
                PhoneNumber = updateUserPayload.PhoneNumber,
                PictureUrl = pictureUrl,
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

    public async Task<ServiceResponse> UpdateUserRatingAsync(UserRating userRating)
    {
        try
        {
            var actualUser = await _userRepository.GetUserAsync(userRating.User.NameIdentifier, userRating.User.Email);

            if (actualUser == null)
            {
                return new ServiceResponse(errorMessage: "User not found");
            }

            var response = await _userRepository.AddOrUpdateUserAsync(new UserEntity()
            {
                PartitionKey = userRating.User.Email,
                RowKey = userRating.User.NameIdentifier,
                UserType = actualUser.UserType,
                PhoneNumber = actualUser.PhoneNumber,
                PictureUrl = actualUser.PictureUrl,
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