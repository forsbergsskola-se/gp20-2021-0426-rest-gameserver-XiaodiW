using System;
using System.IO;
using System.Threading.Tasks;
using MMORPG.Help;
using MMORPG.Types;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MMORPG.APIs {
    
    public class MongoDbRepository : IRepository {
        static readonly MongoClient Client = new MongoClient("mongodb://localhost:27017");
        static readonly IMongoDatabase Database = Client.GetDatabase("game");
        static readonly IMongoCollection<Player> Collection = Database.GetCollection<Player>("players");

        public async Task<Player> Get(Guid id) {
            Player result;
            try {
                var filter = Builders<Player>.Filter.Eq("Id", id);
                result = await Collection.Find(filter).FirstAsync();
            }
            catch(InvalidOperationException) { throw new NotFoundException("Player ID Not Found!"); }
            return result;
        }

        public async Task<Player[]> GetAll() {
            var result = await Collection.Find(new BsonDocument()).ToListAsync();
            return result.ToArray();
        }

        public async Task<Player> Create(NewPlayer newPlayer) {
            var player = new Player(newPlayer.Name);
            await Collection.InsertOneAsync(player);
            return player;
        }

        public async Task<Player> Modify(Guid id, ModifiedPlayer modifiedPlayer) {
            Player result;
            try {
                var filter = Builders<Player>.Filter.Eq("Id", id);
                var update = Builders<Player>.Update.Set("Score", modifiedPlayer.Score);
                await Collection.UpdateOneAsync(filter, update);
                result = await Collection.Find(filter).FirstAsync();
            }
            catch(InvalidOperationException) { throw new NotFoundException("Player ID Not Found!"); }
            return result;
        }

        public async Task<Player> Delete(Guid id) {
            Player result;
            try {
                var filter = Builders<Player>.Filter.Eq("Id", id);
                result = await Collection.Find(filter).FirstAsync();
                await Collection.DeleteOneAsync(filter);
            }
            catch(InvalidOperationException) { throw new NotFoundException("Player ID Not Found!"); }
            return result;
        }

        public async Task<Item> GetItem(Guid playerId, Guid itemId) {
            Item result;
            Player player;
            try {
                var builder = Builders<Player>.Filter;
                var filter = builder.Eq("Id", playerId);
                player = await Collection.Find(filter).FirstAsync();
            }
            catch(InvalidOperationException e) { throw new NotFoundException("Player ID Not Found!"); }
            result = player.Items.Find(item => item.Id == itemId);
            if(result == null) throw new NotFoundException("Item ID Not Found!");
            return result;
        }

        public async Task<Item[]> GetAllItems(Guid playerId) {
            Item[] result;
            var filter = Builders<Player>.Filter.Eq("Id", playerId);
            try {
                var player = await Collection.Find(filter).FirstAsync();
                result = player.Items.ToArray();
            }
            catch(InvalidOperationException e) { throw new NotFoundException("Player ID Not Found!"); }
            return result;
        }

        public async Task<Item> AddItem(Guid playerId, NewItem item) {
            var builder = Builders<Player>.Filter;
            var filter = builder.Eq(x => x.Id, playerId);
            try { await Collection.Find(filter).FirstAsync(); }
            catch(Exception e) { throw new NotFoundException("Player ID Not Found!"); }
            var result = new Item(item.Name,item.Type);
            var update = Builders<Player>.Update.AddToSet(x => x.Items, result);
            await Collection.UpdateOneAsync(filter, update);
            return result;
        }

        public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
            Player player;
            var builder = Builders<Player>.Filter;
            var filter = builder.Eq("Id", playerId);
            try { player = await Collection.Find(filter).FirstAsync(); }
            catch(InvalidOperationException e) { throw new NotFoundException("Player ID Not Found!"); }
            var result = player.Items.Find(item => item.Id == itemId);
            if(result == null) throw new NotFoundException("Item ID Not Found!");
            var update = Builders<Player>.Update.PullFilter("Items", Builders<Item>.Filter.Eq(x=> x.Id, itemId));
            await Collection.UpdateOneAsync(filter, update);
            return result;
        }
    }

}