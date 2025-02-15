﻿using System.Threading.Tasks;
using Azure;
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
        var user = await _userTable.GetAsync(email, nameIdentifier);
        if (user is not { HasValue: true })
        {
            return null;
        }
        
        return user.Value;
    }

    public async Task<Response> AddOrUpdateUserAsync(UserEntity user)
    {
        var response = await _userTable.AddOrUpdateAsync(user);

        return response;
    }
}