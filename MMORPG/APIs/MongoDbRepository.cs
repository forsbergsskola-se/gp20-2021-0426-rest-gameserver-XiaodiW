using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MMORPG.Controllers;
using MMORPG.Filters;
using MMORPG.Help;
using MMORPG.Types;
using MMORPG.Types.Item;
using MMORPG.Types.Player;
using MMORPG.Types.Quest;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
                .Set(p=>p.Quests, quests)
                .Set(p=>p.LastGetQuests, DateTime.Now);
            await UpdatePlayer(id, update);
            player = await GetPlayer(id);
            return player;
        }

        public async Task<Player> DoQuests(Guid id, Guid questId) {
            var player = await GetPlayer(id);
            var quest = Quest.DoQuest(player,questId);
            if(player.Level < quest.Level) throw new GameRestrictionException("Player Level not match!");
            
            var items = player.Items;
            if(quest.Item !=null) items.Add(quest.Item);
            
            var quests = player.Quests;
            quests.Where(w=> w.Id == questId).ToList().ForEach(s=>s.Status = QuestStatus.Done);
            
            var update = Builders<Player>.Update
                .Set(p=>p.Experience, player.Experience + quest.Experience)
                .Set(p=>p.Gold, player.Gold + quest.Gold)
                .Set(p=>p.Items, items)
                .Set(p=>p.Quests, quests);
            await UpdatePlayer(id, update);
            player = await GetPlayer(id);
            return player;
        }

        public async Task<Player> UpgradeLevel(Guid id, int gold) {
            var player = await GetPlayer(id);
            var count = player.Gold / 100;
            if(count <= 0 || player.Gold < gold) 
                throw new GameRestrictionException("Player has no sufficient Gold to Upgrade Level");
            var modifiedPlayer = new ModifiedPlayer();
            modifiedPlayer.Gold = player.Gold - gold;
            modifiedPlayer.Level = player.Level + count;
            player = await Modify(id, modifiedPlayer);
            return player;
        }

        public async Task<Player> UpgradeLevel(Guid id) {
            var player = await GetPlayer(id);
            var count = player.Experience / 100;
            if(count <= 0) throw new GameRestrictionException("Player has no sufficient Exp to Upgrade Level");
            var modifiedPlayer = new ModifiedPlayer();
            modifiedPlayer.Experience = player.Experience - count * 100;
            modifiedPlayer.Level = player.Level + count;
            player = await Modify(id, modifiedPlayer);
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
            if(item.Type == 0 && player.Level < 3) throw new NewItemValidationException();
            var result = new Item(player.Level);
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

        public async Task<Item> HandleItem(Guid id, Guid itemId,ItemActions actions) {
            var player = await GetPlayer(id);
            var items = player.Items.ToList();
            var result = items.Find(item => item.Id == itemId);
            if(result == null) throw new NotFoundException("Item ID Not Found!");
            UpdateDefinition<Player> update = null;
            switch(actions) {
                case ItemActions.Sell: 
                    if(result.IsEquipped)  throw new GameRestrictionException("Item cannot be Sold, have to be unequipped first.");
                    items.Where(i=>i.Id == itemId).ToList().ForEach(i=>i.IsDeleted = true);
                    update = Builders<Player>.Update
                        .Set(p => p.Gold, player.Gold + result.Price)
                        .Set(p => p.Items, items);
                    break;
                case ItemActions.Equip:
                    var equippedSameType = items.Any(i => i.IsEquipped && i.Type == result.Type);
                    if(equippedSameType)  
                        throw new GameRestrictionException("Item cannot be equipped, the same type item has to be unequipped first.");
                    if(player.Level < result.LevelRequired)
                        throw new GameRestrictionException("Item cannot be equipped, Player Level not match requirement.");
                    items.Where(i=>i.Id == itemId).ToList().ForEach(i=>i.IsEquipped = true);
                    update = Builders<Player>.Update
                        .Set(p => p.Level, player.Level + result.LevelBonus)
                        .Set(p => p.Items, items);
                    break;
                case ItemActions.Unequip:
                    var newLevel = player.Level - result.LevelBonus;
                    items.Where(i=>i.Id == itemId || newLevel <i.LevelRequired).ToList().ForEach(i=>i.IsEquipped = false);
                    update = Builders<Player>.Update
                        .Set(p => p.Level, newLevel)
                        .Set(p => p.Items, items);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(actions), actions, null);
            }
            await UpdatePlayer(id, update);
            return result;
        }

        public async Task<Player[]> GetLeaderBoard(LeaderBoardOrderBy orderBy) {
            var filter = SimpleFilter(null, null, null);
            SortDefinition<Player> sort = null;
            switch(orderBy) {
                case LeaderBoardOrderBy.Level:
                    sort = Builders<Player>.Sort.Descending(p => p.Level);
                    break;
                case LeaderBoardOrderBy.Gold: 
                    sort = Builders<Player>.Sort.Descending(p => p.Gold);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null);
            }
            var response = await Collection.Find(filter).Sort(sort).Limit(10).ToListAsync();
            var restriction = new[] {"Name", "Level", "Gold"};
            var result = ComApi.RestrictedData(response,restriction).ToArray();
            return result;

        }

        public async Task<long> GetStats(StatsRequest request) {
            long result;
            switch(request) {
                case StatsRequest.PlayerCount:
                    var filter = SimpleFilter(null, null, null);
                    result = await Collection.CountDocumentsAsync(filter);
                    break;
                case StatsRequest.TotalGold: 
                    result = await Collection.AsQueryable().SumAsync(p=>p.Gold);
                    break;
                case StatsRequest.TotalItem: 
                    result = await Collection.AsQueryable().SumAsync(p=>p.Items.Count);
                    break;
                case StatsRequest.TotalLevel: 
                    result = await Collection.AsQueryable().SumAsync(p=>p.Level);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(request), request, null);
            }
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