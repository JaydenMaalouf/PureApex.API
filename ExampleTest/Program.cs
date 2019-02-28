using System;
using System.Linq;
using System.Threading.Tasks;

using ApexLegendsAPI;

namespace ExampleTest
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            var api = new ApexAPI("username", "password");
            var result = await api.LoginAsync();
            if (result)
            {
                var self = api.ApexUser;
                Console.WriteLine("--- Self ---");
                Console.WriteLine($"Id: {self.UserId}");
                Console.WriteLine($"Username: {self.Username}");
                Console.WriteLine($"PersonaId: {self.PersonaId}");

                Console.WriteLine();
                Console.WriteLine();

                var user = await api.GetUserAsync("Munkeed");
                if (user != null)
                {
                    Console.WriteLine("--- Munkeed ---");
                    Console.WriteLine($"Id: {user.UserId}");
                    Console.WriteLine($"Username: {user.Username}");
                    Console.WriteLine($"PersonaId: {user.PersonaId}");
                    Console.WriteLine($"AvatarUrl: {await user.GetAvatarUrlAsync()}");

                    var stats = await user.GetStatsAsync();
                    if (stats != null)
                    {
                        Console.WriteLine($"IsOnline: {stats.IsOnline}");
                        Console.WriteLine($"IsJoinable: {stats.IsJoinable}");
                        Console.WriteLine($"IsInMatch: {stats.PlayerInMatch}");
                        Console.WriteLine($"Character: {stats.Character.Type}");
                        Console.WriteLine($"Banner 1: {stats.Banner1.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner1.TrackerValue}");
                        Console.WriteLine($"Banner 1: {stats.Banner2.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner2.TrackerValue}");
                        Console.WriteLine($"Banner 1: {stats.Banner3.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner3.TrackerValue}");
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                }

                var users = await api.GetUsersAsync("Munk", 2);
                Console.WriteLine($"--- Search (Munk) User(s) ---");
                Console.WriteLine($"Count: {users.Count()}");
                foreach (var searchUser in users)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Id: {searchUser.UserId}");
                    Console.WriteLine($"Username: {searchUser.Username}");
                    Console.WriteLine($"PersonaId: {searchUser.PersonaId}");
                    Console.WriteLine($"AvatarUrl: {await searchUser.GetAvatarUrlAsync()}");

                    var stats = await searchUser.GetStatsAsync();
                    if (stats != null)
                    {
                        Console.WriteLine($"IsOnline: {stats.IsOnline}");
                        Console.WriteLine($"IsJoinable: {stats.IsJoinable}");
                        Console.WriteLine($"IsInMatch: {stats.PlayerInMatch}");
                        Console.WriteLine($"Character: {stats.Character.Type}");
                        Console.WriteLine($"Banner 1: {stats.Banner1.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner1.TrackerValue}");
                        Console.WriteLine($"Banner 1: {stats.Banner2.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner2.TrackerValue}");
                        Console.WriteLine($"Banner 1: {stats.Banner3.TrackerType}");
                        Console.WriteLine($"Banner 1 Value: {stats.Banner3.TrackerValue}");
                    }
                }
            }
            Console.ReadLine();
        }        
    }
}
