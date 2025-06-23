using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Contracts.SignalRModels;
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

    public async Task<ServiceResponse<User>> GetUserAsync(UserIdentifier user, string name = default)
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
                    UserName = name,
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
                UserName = actualUser.UserName,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<User>(ex);
        }
    }

    public ServiceResponse<List<User>> GetUsers()
    {
        try
        {
            var users = _userRepository.GetUsers();

            return new ServiceResponse<List<User>>(users.Select(x => new User()
            {
                UserType = (Server.Contracts.Enums.UserType)x.UserType,
                Identifier = new UserIdentifier()
                {
                    NameIdentifier = x.RowKey,
                    Email = x.PartitionKey,
                },
                Rating = x.Rating,
                PhoneNumber = x.PhoneNumber,
                PictureUrl = x.PictureUrl,
                UserName = x.UserName,
            }).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<List<User>>(ex);
        }
    }

    public async Task<ServiceResponse<User>> GetUserByUserIdAsync(string userId)
    {
        try
        {
            var actualUser = await _userRepository.GetUserByUserNameIdentifierAsync(userId);

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
                UserName = actualUser.UserName,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ServiceResponse<User>(ex);
        }
    }

    public async Task<ServiceResponse> UpdateUserAsync(UpdateUserPayload updateUserPayload)
    {
        try
        {
            var actualUser =
                await _userRepository.GetUserAsync(updateUserPayload.User.NameIdentifier, updateUserPayload.User.Email);

            if (actualUser == null)
            {
                return new ServiceResponse(errorMessage: "User not found");
            }

            var response = await _userRepository.AddOrUpdateUserAsync(new UserEntity()
            {
                PartitionKey = actualUser.PartitionKey,
                RowKey = actualUser.RowKey,
                UserType = updateUserPayload.UserType == null
                    ? actualUser.UserType
                    : (UserType)updateUserPayload.UserType,
                Rating = actualUser.Rating,
                PhoneNumber = !string.IsNullOrEmpty(updateUserPayload.PhoneNumber)
                    ? updateUserPayload.PhoneNumber
                    : actualUser.PhoneNumber,
                PictureUrl = !string.IsNullOrEmpty(updateUserPayload.PictureUrl)
                    ? updateUserPayload.PictureUrl
                    : actualUser.PictureUrl,
                UserName = actualUser.UserName,
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

    public async Task<ServiceResponse> UpdateUserRatingAsync(string userId, int rating)
    {
        try
        {
            var actualUser = await _userRepository.GetUserByUserNameIdentifierAsync(userId);

            if (actualUser == null)
            {
                return new ServiceResponse(errorMessage: "User not found");
            }

            actualUser.Rating = CalculateRating(actualUser.Rating, rating);
            
            var response = await _userRepository.AddOrUpdateUserAsync(actualUser);

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