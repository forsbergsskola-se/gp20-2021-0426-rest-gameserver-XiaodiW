using System;
using System.Threading.Tasks;
using MMORPG.Filters;
using MMORPG.Help;
using MMORPG.Types;
using MongoDB.Driver;

namespace MMORPG.APIs {

    [ValidateExceptionFilter]
    public class MongoDbRepository : IRepository {
        private static readonly MongoClient Client = new("mongodb://localhost:27017");
        private static readonly IMongoDatabase Database = Client.GetDatabase("game");
        private static readonly IMongoCollection<Player> Collection = Database.GetCollection<Player>("players");

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
            var filter = Builders<Player>.Filter.Eq(p => p.IsDeleted, false);
            var response = await Collection.Find(filter).ToListAsync();
            var restriction = new[] {"Name"};
            var restrictedResult = ComApi.RestrictedData(response, restriction);
            var result = restrictedResult.ToArray();
            return result;
        }
        public async Task<Player[]> AdminGetAll() {
            var filter = Builders<Player>.Filter.Eq(p => p.IsDeleted, false);
            var response = await Collection.Find(filter).ToListAsync();
            var result = response.ToArray();
            return result;
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
                foreach(var field in modifiedPlayer.GetType().GetProperties()) {
                    var fieldValue = (int) modifiedPlayer.GetType().GetProperty(field.Name).GetValue(modifiedPlayer);
                    if(fieldValue < 0) continue;
                    var update = Builders<Player>.Update.Set(field.Name, fieldValue);
                    await Collection.UpdateOneAsync(filter, update);
                }
                result = await Collection.Find(filter).FirstAsync();
            }
            catch(InvalidOperationException) { throw new NotFoundException("Player ID Not Found!"); }
            return result;
        }

        public async Task<Player> MarkDelete(Guid id) {
            Player result;
            try {
                var filter = Builders<Player>.Filter.Eq("Id", id);
                var update = Builders<Player>.Update.Set("IsDeleted", true);
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

        public async Task<Player[]> GetScoreGt(int minScore) {
            Player[] result;
            var filter = Builders<Player>.Filter.Gt(p => p.Score, minScore);
            filter &= Builders<Player>.Filter.Eq(p => p.IsDeleted, false);
            try {
                var data = await Collection.Find(filter).ToListAsync();
                result = data.ToArray();
            }
            catch(InvalidOperationException) { throw new NotFoundException("Required Player Not Found!"); }
            return result;
        }

        public async Task<Player[]> GetPlayerByName(string name) {
            Player[] result;
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            filter &= Builders<Player>.Filter.Eq(p => p.IsDeleted, false);
            try {
                var data = await Collection.Find(filter).ToListAsync();
                result = ComApi.RestrictedData(data,new []{"Name","Score","Level","Gold","items"}).ToArray();
            }
            catch(InvalidOperationException) { throw new NotFoundException("Required Player Not Found!"); }
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
            Player player;
            try { player = await Collection.Find(filter).FirstAsync(); }
            catch(Exception e) { throw new NotFoundException("Player ID Not Found!"); }
            if(item.Type == ItemType.Sword && player.Level < 3) throw new NewItemValidationException();
            var result = new Item(item.Name, item.Type);
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
            var update = Builders<Player>.Update.PullFilter("Items", Builders<Item>.Filter.Eq(x => x.Id, itemId));
            await Collection.UpdateOneAsync(filter, update);
            return result;
        }
    }

}