using System;
using System.Threading.Tasks;
using Azure;
using FastRide.Client;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ITableClient<UserEntity> _userTable;

    public UserRepository(ITableClient<UserEntity> userTable)
    {
        this._userTable = userTable;
    }
    
    public async Task<UserEntity> GetUserAsync(string nameIdentifier, string email)
    {
        if (!Guid.TryParse(nameIdentifier, out var id))
        {
            id = nameIdentifier.GenerateGuidFromString();
        }
        var user = await _userTable.GetAsync(email, id.ToString());
        return user is not { HasValue: true } ? null : user.Value;
    }

    public async Task<Response> AddOrUpdateUserAsync(UserEntity user)
    {
        if (!Guid.TryParse(user.RowKey, out var id))
        {
            id = user.RowKey.GenerateGuidFromString();
        }

        user.RowKey = id.ToString();

        var response = await _userTable.AddOrUpdateAsync(user);

        return response;
    }
}