using System.Threading.Tasks;

using Pure.Apex.API.Classes.Stats;

namespace Pure.Apex.API.Interfaces
{
    public interface IApexUser
    {
        Task<ApexUserStats> GetStatsAsync(PlatformType platformType);
    }
}
