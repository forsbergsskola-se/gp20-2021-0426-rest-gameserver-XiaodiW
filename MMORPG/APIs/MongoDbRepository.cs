using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMORPG.Filters;
using MMORPG.Help;
using MMORPG.Types.Item;
using MMORPG.Types.Player;
using MMORPG.Types.Quest;
using MongoDB.Driver;

namespace MMORPG.APIs {

    [ValidateExceptionFilter]
    public class MongoDbRepository : IRepository {
        private static readonly MongoClient Client = new("mongodb://localhost:27017");
        private static readonly IMongoDatabase Database = Client.GetDatabase("game");
        private static readonly IMongoCollection<Player> Collection = Database.GetCollection<Player>("players");

        public async Task<Player> Get(Guid id) {
            var player = await GetPlayer(id);
            return player;
        }
        
        public async Task<Player> AdminGet(Guid id) {
            var filter = SimpleFilter(id, null, null);
            var player = await GetPlayer(filter);
            return player.FirstOrDefault();
        }

        public async Task<Player[]> GetAll() {
            var filter = SimpleFilter(null, null, false);
            var response = await Collection.Find(filter).ToListAsync();
            var restrictedResult = ComApi.RestrictedData(response, new[] {"Name"});
            var result = restrictedResult.ToArray();
            return result;
        }
        public async Task<Player[]> AdminGetAll() {
            var filter = SimpleFilter(null, null, null);
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
            var update = Builders<Player>.Update;
            var updates = new List<UpdateDefinition<Player>>();
            foreach(var property in modifiedPlayer.GetType().GetProperties()) {
                var value = property.GetValue(modifiedPlayer);
                if(value != null && (int)value < 0) continue;
                updates.Add(update.Set(property.Name, value));
            }
            await UpdatePlayer(id, update.Combine(updates));
            var player = await GetPlayer(id);
            return player;
        }

        public async Task<Player> MarkDelete(Guid id) {
            var update = Builders<Player>.Update.Set(p=>p.IsDeleted, true);
            await UpdatePlayer(id, update);
            var filter = SimpleFilter(id, null, null);
            var player = await GetPlayer(filter);
            return player.FirstOrDefault();
        }

        public async Task<Player> Delete(Guid id) {
            var player = await GetPlayer(id);
            var filter = SimpleFilter(id,null,null);
            await Collection.DeleteOneAsync(filter);
            return player;
        }

        public async Task<Player[]> GetScoreGt(int minScore) {
            var filter = SimpleFilter(null, null, false);
            filter &= Builders<Player>.Filter.Gt(p => p.Score, minScore);
            var data = await GetPlayer(filter);
            var restriction = new[] {"Name", "Score", "Level", "Gold", "items"};
            var result = ComApi.RestrictedData(data,restriction).ToArray();
            return result;
        }

        public async Task<Player[]> GetPlayerByName(string name) {
            var filter = SimpleFilter(null, name, false);
            var data = await GetPlayer(filter);
            var restriction = new[] {"Name", "Score", "Level", "Gold", "items"};
            var result = ComApi.RestrictedData(data,restriction).ToArray();
            return result;
        }

        public async Task<Player> GetQuests(Guid id) {
            var player = await GetPlayer(id);

            var quests = Quest.GetQuests(player);
            if(player.Quests.Equals(quests)) return player;
            var update = Builders<Player>.Update
                .Set("Quests", quests)
                .Set("LastGetQuests", DateTime.Now);
            await UpdatePlayer(id, update);
            player = await GetPlayer(id);
            return player;
        }

        public async Task<Player> DoQuests(Guid id, Guid questId) {
            var player = await GetPlayer(id);
            
            var quest = Quest.DoQuests(player,questId);
            if(player.Level < quest.Level) throw new GameRestrictionException("Player Level not match!");
            
            var items = player.Items;
            items.Add(quest.Item);
            
            var quests = player.Quests;
            quests.Where(w=> w.Id == questId).ToList().ForEach(s=>s.Status = QuestStatus.Done);
            
            var update = Builders<Player>.Update
                .Set("Experience", player.Experience + quest.Experience)
                .Set("Gold", player.Gold + quest.Gold)
                .Set("Items", items)
                .Set("Quests", quests);
            await UpdatePlayer(id, update);
            player = await GetPlayer(id);
            return player;
        }

        public async Task<Item> GetItem(Guid id, Guid itemId) {
            var player = await GetPlayer(id);
            var result = player.Items.Find(item => item.Id == itemId);
            if(result == null) throw new NotFoundException("Item ID Not Found!");
            return result;
        }

        public async Task<Item[]> GetAllItems(Guid id) {
            var player = await GetPlayer(id);
            var result = player.Items.ToArray();
            return result;
        }

        public async Task<Item> AddItem(Guid id, NewItem item) {
            var player = await GetPlayer(id);
            if(item.Type == ItemType.Sword && player.Level < 3) throw new NewItemValidationException();
            var result = new Item(item.Name, item.Type);
            var update = Builders<Player>.Update.AddToSet(x => x.Items, result);
            await UpdatePlayer(id,update);
            return result;
        }

        public async Task<Item> DeleteItem(Guid id, Guid itemId) {
            var player = await GetPlayer(id);
            var result = player.Items.Find(item => item.Id == itemId);
            if(result == null) throw new NotFoundException("Item ID Not Found!");
            var update = Builders<Player>.Update.PullFilter("Items", Builders<Item>.Filter.Eq(x => x.Id, itemId));
            await UpdatePlayer(id,update);
            return result;
        }
        private static async Task<Player> GetPlayer(Guid id) {
            Player player;
            var filter = SimpleFilter(id,null,false);
            try { player = await Collection.Find(filter).FirstAsync(); }
            catch(InvalidOperationException e) { throw new NotFoundException("Player ID Not Found!"); }
            return player;
        }
        
        private static async Task<List<Player>> GetPlayer(FilterDefinition<Player> filter) {
            List<Player> player;
            try { player = await Collection.Find(filter).ToListAsync();}
            catch(InvalidOperationException e) { throw new NotFoundException("Player Name Not Found!"); }
            return player;
        }
        
        private static async Task UpdatePlayer(Guid id,UpdateDefinition<Player> update) {
            var filter = SimpleFilter(id,null,false);
            try { await Collection.Find(filter).FirstAsync(); }
            catch(InvalidOperationException e) { throw new NotFoundException("Player ID Not Found!"); }
            await Collection.UpdateOneAsync(filter, update);
        }

        private static FilterDefinition<Player> SimpleFilter(Guid? id, string name, bool? deleted) {
            var filter = Builders<Player>.Filter.Empty;
            if(id != null) filter &= Builders<Player>.Filter.Eq(p => p.Id, id);
            if(name != null) filter &= Builders<Player>.Filter.Eq(p => p.Name, name);
            if(deleted != null) filter &= Builders<Player>.Filter.Eq(p => p.IsDeleted, deleted);
            return filter;
        }
    }

}