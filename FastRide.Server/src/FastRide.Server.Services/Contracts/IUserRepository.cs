using System.Threading.Tasks;
using Azure;
using FastRide_Server.Services.Entities;
using FastRide_Server.Services.Enums;

namespace FastRide_Server.Services.Contracts;

public interface IUserRepository
{
    Task<UserType?> GetUserType(string nameIdentifier, string email);
    Task<Response> RegisterUser(UserEntity user);
}