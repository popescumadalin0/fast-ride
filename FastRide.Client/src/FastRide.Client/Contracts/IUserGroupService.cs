using System.Threading.Tasks;

namespace FastRide.Client.Contracts;

public interface IUserGroupService
{
    /// <summary>
    /// Get user group name for the current user
    /// </summary>
    /// <returns></returns>
    Task<string> GetCurrentUserGroupNameAsync();
}