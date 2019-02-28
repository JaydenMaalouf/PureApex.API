using System.Threading.Tasks;

using PureApexAPI.Classes.Stats;

namespace PureApexAPI.Interfaces
{
    public interface IApexUser
    {
        Task<ApexUserStats> GetStatsAsync(PlatformType platformType);
    }
}
