using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MMORPG.Types;
using System.Text.Json;

namespace MMORPG.APIs {

    public class FileRepository : IRepository {
        private static readonly string fileName = "player.json";
        private readonly string _path = Path.Combine(Environment.CurrentDirectory, @"OfflineData", fileName);

        private async Task<List<Player>> ReadFile() {
            try {
                using var sr = new StreamReader(_path);
                var data = await sr.ReadToEndAsync();
                var result = JsonSerializer.Deserialize<List<Player>>(data);
                sr.Close();
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<Player> Get(Guid id) {
            var data = await ReadFile();
            var result = data.FirstOrDefault(a => a.Id == id);
            return result;
        }

        public async Task<Player[]> GetAll() {
            var result = await ReadFile();
            if(result == null) result = new List<Player>();
            return result.ToArray();
        }

        public async Task<Player> Create(NewPlayer newPlayer) {
            var players = await ReadFile();
            if(players == null) players = new List<Player>();
            var player = new Player(newPlayer.Name);
            players.Add(player);
            try {
                await using var createStream = File.OpenWrite(_path);
                await JsonSerializer.SerializeAsync(createStream, players);
                createStream.Close();
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return player;
        }

        public async Task<Player> Modify(Guid id, ModifiedPlayer modifiedPlayer) {
            var result = new Player(null);
            try {
                var allPlayers = await ReadFile();
                var index = -1;
                for(var i = 0; i < allPlayers.Count; i++)
                    if(allPlayers[i].Id == id)
                        index = i;
                allPlayers[index].Score = modifiedPlayer.Score;
                result = allPlayers[index];
                await using var createStream = File.Create(_path);
                await JsonSerializer.SerializeAsync(createStream, allPlayers);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<Player> Delete(Guid id) {
            Player result = null;
            try {
                var allPlayers = await ReadFile();
                for(var i = 0; i < allPlayers.Count; i++)
                    if(allPlayers[i].Id == id)
                        result = allPlayers[i];
                allPlayers.Remove(result);
                await using var createStream = File.Create(_path);
                await JsonSerializer.SerializeAsync(createStream, allPlayers);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<Item> GetItem(Guid playerId, Guid itemId) {
            Item result = null;
            try {
                var data = await ReadFile();
                var player = data.FirstOrDefault(a => a.Id == playerId);
                if(player == null) throw new NullReferenceException("The Player ID is not exist.");
                result = player.Items.Find(a => a.Id == itemId);
                if(result == null) throw new NullReferenceException("The Item ID is not exist.");
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<Item[]> GetAllItems(Guid playerId) {
            Item[] result = null;
            try {
                var data = await ReadFile();
                var player = data.FirstOrDefault(a => a.Id == playerId);
                if(player == null) throw new NullReferenceException("The player ID is not exist.");
                result = player.Items.ToArray();
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<Item> AddItem(Guid playerId, NewItem item) {
            Item result = null;
            try {
                var data = await ReadFile();
                var player = data.FirstOrDefault(a => a.Id == playerId);
                if(player == null) throw new NullReferenceException("The player ID is not exist.");
                result = new Item(item.Name);
                player.Items.Add(result);
                await using var createStream = File.Create(_path);
                await JsonSerializer.SerializeAsync(createStream, data);
                createStream.Close();
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
            Item result = null;
            try {
                var data = await ReadFile();
                var player = data.FirstOrDefault(a => a.Id == playerId);
                if(player == null) throw new NullReferenceException("The Player ID is not exist.");
                result = player.Items.Find(a => a.Id == itemId);
                if(result == null) throw new NullReferenceException("The Item ID is not exist.");
                player.Items.Remove(result);
                await using var createStream = File.Create(_path);
                await JsonSerializer.SerializeAsync(createStream, data);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }

}