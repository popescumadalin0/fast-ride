using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Contracts;

public interface IUserRepository
{
    Task<UserEntity> GetUserAsync(string nameIdentifier, string email);
    
    List<UserEntity> GetUsers();

    Task<UserEntity> GetUserByUserNameIdentifierAsync(string nameIdentifier);
    
    Task<Response> AddOrUpdateUserAsync(UserEntity user);
}