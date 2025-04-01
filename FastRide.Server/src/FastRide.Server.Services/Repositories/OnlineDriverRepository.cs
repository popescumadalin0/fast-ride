using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using FastRide.Server.Services.Contracts;
using FastRide.Server.Services.Entities;

namespace FastRide.Server.Services.Repositories;

public class OnlineDriverRepository : IOnlineDriverRepository
{
    private readonly ITableClient<OnlineDriversEntity> _onlineDriverTable;

    public OnlineDriverRepository(ITableClient<OnlineDriversEntity> onlineDriverTable)
    {
        this._onlineDriverTable = onlineDriverTable;
    }

    public Task<List<OnlineDriversEntity>> GetOnlineDriversByGroupNameAsync(string groupName)
    {
        var rides = _onlineDriverTable.GetBy(x => x.PartitionKey == groupName);

        return Task.FromResult(rides);
    }

    public Task<OnlineDriversEntity> GetOnlineDriversByUserIdAsync(string userId)
    {
        var rides = _onlineDriverTable.GetBy(x => x.RowKey == userId);

        return Task.FromResult(rides.SingleOrDefault());
    }

    public async Task<Response> AddOnlineDriverAsync(OnlineDriversEntity ride)
    {
        var response = await _onlineDriverTable.AddOrUpdateAsync(ride);

        return response;
    }

    public async Task<Response> DeleteOnlineDriverAsync(string groupName, string userId)
    {
        var response = await _onlineDriverTable.DeleteAsync(groupName, userId);

        return response;
    }
}