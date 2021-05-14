using System;
using System.Data.Common;
using System.Threading.Tasks;
using MMORPGClient.APIs;

namespace MMORPGClient
{
    class Program {
        private static Guid _playerId;
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Type in new Player name");
            var name = Console.ReadLine();
            var player = await Player.CreatePlayer(name);
            _playerId = player.Id;
            Console.WriteLine($"New Player Created: Player Name: {player.Name}; PLayer ID:{player.Id}");
            Console.WriteLine("Do you want to delete your self? Yes/No?");
            var s = Console.ReadLine().ToLower();
            if(s == "y") await Player.DeletePlayer(_playerId);
        }
    }
}
