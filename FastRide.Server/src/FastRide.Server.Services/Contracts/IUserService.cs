using System.Threading.Tasks;
using FastRide_Server.Services.Enums;
using FastRide_Server.Services.Models;

namespace FastRide_Server.Services.Contracts;

public interface IUserService
{
    Task<ServiceResponse<UserType>> GetUserType(string nameIdentifier, string email);
}