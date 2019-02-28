using System.Threading.Tasks;

using PureApex.API.Classes.Stats;

namespace PureApex.API.Interfaces
{
    public interface IApexUser
    {
        Task<ApexUserStats> GetStatsAsync(PlatformType platformType);
    }
}
