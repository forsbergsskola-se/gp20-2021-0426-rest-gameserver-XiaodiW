using System;
using System.Threading.Tasks;
using MMORPG.Types;

namespace MMORPG.APIs {

    public interface IRepository
    {
        Task<Player> Get(Guid id);
        Task<Player[]> GetAll();
        Task<Player[]> AdminGetAll();
        Task<Player> Create(NewPlayer newPlayer);
        Task<Player> Modify(Guid id, ModifiedPlayer modifiedPlayer);
        Task<Player> MarkDelete(Guid id);
        Task<Player> Delete(Guid id);

        Task<Player[]> GetScoreGt(int minScore);
        Task<Player[]> GetPlayerByName(string name);
        

        Task<Item> GetItem(Guid playerId, Guid itemId);
        Task<Item[]> GetAllItems(Guid playerId);
        Task<Item> AddItem(Guid playerId, NewItem item);
        Task<Item> DeleteItem(Guid playerId, Guid itemId);

    }

}