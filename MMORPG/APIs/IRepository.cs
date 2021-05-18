using System;
using System.Threading.Tasks;
using MMORPG.Types;
using MMORPG.Types.Item;
using MMORPG.Types.Player;

namespace MMORPG.APIs {

    public interface IRepository
    {
        Task<Player> Get(Guid id);
        Task<Player> AdminGet(Guid id);
        Task<Player[]> GetAll();
        Task<Player[]> AdminGetAll();
        Task<Player> Create(NewPlayer newPlayer);
        Task<Player> Modify(Guid id, ModifiedPlayer modifiedPlayer);
        Task<Player> MarkDelete(Guid id);
        Task<Player> Delete(Guid id);

        Task<Player[]> GetScoreGt(int minScore);
        Task<Player[]> GetPlayerByName(string name);
        
        Task<Player> GetQuests(Guid id);
        Task<Player> DoQuests(Guid id,Guid questId);

        Task<Player> UpgradeLevel(Guid id,int gold);
        Task<Player> UpgradeLevel(Guid id);
        
        Task<Item> GetItem(Guid id, Guid itemId);
        Task<Item[]> GetAllItems(Guid id);
        Task<Item> AddItem(Guid id, NewItem item);
        Task<Item> DeleteItem(Guid id, Guid itemId);

        Task<Item> HandleItem(Guid id, Guid itemId, ItemActions action);

        Task<Player[]> GetLeaderBoard(LeaderBoardOrderBy orderBy);

    }

}