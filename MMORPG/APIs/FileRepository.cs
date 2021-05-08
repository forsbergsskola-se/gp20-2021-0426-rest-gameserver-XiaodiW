using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MMORPG.Types;

namespace MMORPG.APIs {

    public class FileRepository : IRepository {
        private static readonly string fileName = "player.json";
        private static readonly string fileNameItem = "item.json";
        private readonly string path = Path.Combine(Environment.CurrentDirectory, @"OffineData", fileName);
        private readonly string itemPath = Path.Combine(Environment.CurrentDirectory, @"OffineData", fileNameItem);


        private async Task<List<Player>> ReadFile() {
            try {
                using var sr = new StreamReader(path);
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
            return result.ToArray();
        }

        public async Task<Player> Create(NewPlayer newPlayer) {
            var players = await ReadFile();
            var player = new Player(newPlayer.Name);
            players.Add(player);
            try {
                await using var createStream = File.OpenWrite(path);
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
                await using var createStream = File.Create(path);
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
                await using var createStream = File.Create(path);
                await JsonSerializer.SerializeAsync(createStream, allPlayers);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }

        private async Task<List<Item>> ReadItemFile() {
            try {
                using var sr = new StreamReader(itemPath);
                var data = await sr.ReadToEndAsync();
                var result = JsonSerializer.Deserialize<List<Item>>(data);
                sr.Close();
                return result;
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<Item> GetItem(Guid playerId, Guid itemId) {
            var data = await ReadItemFile();
            var result = data.FirstOrDefault(a => a.Id == itemId && a.Player.Id == playerId);
            return result;        
        }

        public async Task<Item[]> GetAllItems(Guid playerId) {
            var data = await ReadItemFile();
            var result = data.Where(a => a.Player.Id == playerId).ToArray();
            return result;
            
        }

        public async Task<Item> CreateItem(Guid playerId, NewItem item) {
            var items = await ReadItemFile();
            if(items == null) items = new List<Item>();
                var players = await ReadFile();
            var player = players.FirstOrDefault(a => a.Id == playerId);
            var newItem = new Item(player,item.Name);
            items.Add(newItem);
            try {
                await using var createStream = File.OpenWrite(itemPath);
                await JsonSerializer.SerializeAsync(createStream, items);
                createStream.Close();
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return newItem;
        }

        public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
            Item result = null;
            try {
                var allItems = await ReadItemFile();
                for(var i = 0; i < allItems.Count; i++)
                    if(allItems[i].Id == itemId)
                        result = allItems[i];
                allItems.Remove(result);
                await using var createStream = File.Create(itemPath);
                await JsonSerializer.SerializeAsync(createStream, allItems);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }

}