using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApexLegendsAPI.Interfaces
{
    interface IApexAPI
    {
        ApexUser ApexUser { get; }

        Task<bool> LoginAsync();

        Task<bool> LogoutAsync();

        Task<ApexUser> GetUserAsync(string username, bool explicitUsername);

        Task<IEnumerable<ApexUser>> GetUsersAsync(string username, int count);
    }
}
