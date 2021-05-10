using System;
using System.IO;
using System.Threading.Tasks;
using MMORPG.Types;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMORPG.APIs {
    
    public class MongoDbRepository : IRepository {
        static readonly MongoClient Client = new MongoClient("mongodb://localhost:27017");
        static readonly IMongoDatabase Database = Client.GetDatabase("game");
        static readonly IMongoCollection<Player> Collection = Database.GetCollection<Player>("players");

        public async Task<Player> Get(Guid id) {
            var filter = Builders<Player>.Filter.Eq("Id", id);
            var result = await Collection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<Player[]> GetAll() {
            var result = await Collection.Find(new BsonDocument()).ToListAsync();
            return result.ToArray();
        }

        public async Task<Player> Create(NewPlayer newPlayer) {
            var player = new Player(newPlayer.Name);
            try {
                await Collection.InsertOneAsync(player);
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
                var filter = Builders<Player>.Filter.Eq("Id", id);
                var update = Builders<Player>.Update.Set("Score", modifiedPlayer.Score);
                await Collection.UpdateOneAsync(filter, update);
                result = await Collection.Find(filter).FirstAsync();
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
                var filter = Builders<Player>.Filter.Eq("Id", id);
                result = await Collection.Find(filter).FirstAsync();
                await Collection.DeleteOneAsync(filter);
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
                var builder = Builders<Player>.Filter;
                var filter = builder.Eq("Id", playerId) & builder.Eq("Items.Id", itemId);
                var player = await Collection.Find(filter).FirstAsync();
                result = player.Items.Find(item => item.Id == itemId);
                return result;
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
                var filter = Builders<Player>.Filter.Eq("Id", playerId);
                var player = await Collection.Find(filter).FirstAsync();
                result = player.Items.ToArray();
                return result;
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
                var builder = Builders<Player>.Filter;
                var filter = builder.Eq(x => x.Id, playerId);
                result = new Item(new Player(""), item.Name);
                var update = Builders<Player>.Update.AddToSet(x => x.Items,result);
                await Collection.UpdateOneAsync(filter, update);
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
                var builder = Builders<Player>.Filter;
                var filter =  builder.Eq("Id", playerId) & builder.Eq("Items.Id", itemId);
                var player = await Collection.Find(filter).FirstAsync();
                result = player.Items.Find(item => item.Id == itemId);
                var update = Builders<Player>.Update.PullFilter("Items", Builders<Item>.Filter.Eq(x=> x.Id, itemId));
                await Collection.UpdateOneAsync(filter, update);
            }
            catch(IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return result;
        }
    }

}