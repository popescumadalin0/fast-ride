using System.Collections.Generic;
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

    public async Task<Response> AddOnlineDriverAsync(OnlineDriversEntity ride)
    {
        var response = await _onlineDriverTable.AddOrUpdateAsync(ride);

        return response;
    }
}