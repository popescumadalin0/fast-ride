using System.Threading.Tasks;
using FastRide.Server.Services.Enums;
using FastRide.Server.Services.Models;

namespace FastRide.Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<UserType>> GetUserType(string nameIdentifier, string email);
}