using System.Threading.Tasks;

using ApexLegendsAPI.Classes.Stats;

namespace ApexLegendsAPI.Interfaces
{
    public interface IApexUser
    {
        string Username { get; }
        string UserId { get; }
        string PersonaId { get; }

        Task<ApexUserStats> GetStatsAsync(PlatformType platformType);
    }
}
