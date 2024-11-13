using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;
using FastRide.Server.Services.Enums;

namespace FastRide.Server.Services.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ITableClient<UserEntity> _userTable;

    public UserRepository(ITableClient<UserEntity> userTable)
    {
        this._userTable = userTable;
    }

    public async Task<UserType?> GetUserType(string nameIdentifier, string email)
    {
        var user = await _userTable.GetAsync(nameIdentifier, email);
        if (!user.HasValue)
        {
            return null;
        }

        return user.Value.UserType;
    }

    public async Task<Response> RegisterUser(UserEntity user)
    {
        var response = await _userTable.AddOrUpdateAsync(user);

        return response;
    }
}