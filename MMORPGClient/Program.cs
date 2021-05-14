using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using MMORPGClient.APIs;

namespace MMORPGClient
{
    class Program {
        private static Guid _playerId;
        private static async Task Main(string[] args)
        {
            // Console.WriteLine("Type in new Player name");
            // var name = Console.ReadLine();
            // var player = await Player.CreatePlayer(name);
            // _playerId = player.Id;
            // Console.WriteLine($"New Player Created: Player Name: {player.Name}; PLayer ID:{player.Id}");
            var all = await Player.ListAllPlayers();
            Console.WriteLine($"There are total {all.Length} Players");
            for(var i = 0; i < all.Length; i++) {
                var endStr = i % 7 == 0 ? "\r\n" : String.Empty;
                Console.Write($"{all[i].Name.PadRight(25)}{endStr}");
            }
            // Console.WriteLine("\r\nDo you want to delete your self? Yes/No?");
            // var s = Console.ReadLine().ToLower();
            // if(s == "y") await Player.DeletePlayer(_playerId);
        }
    }
}
