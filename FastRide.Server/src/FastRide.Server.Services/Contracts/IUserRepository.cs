using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Contracts;

public interface IUserRepository
{
    Task<UserEntity> GetUserAsync(string nameIdentifier, string email);
    
    Task<Response> AddOrUpdateUserAsync(UserEntity user);
}