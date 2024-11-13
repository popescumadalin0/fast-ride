using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Services.Contracts;

public interface IUserRepository
{
    Task<UserType?> GetUserType(string nameIdentifier, string email);
    Task<Response> RegisterUser(UserEntity user);
}