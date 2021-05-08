using System;
using System.Threading.Tasks;
using MMORPG.Types;

namespace MMORPG.APIs {

    public interface IRepository
    {
        Task<Player> Get(Guid id);
        Task<Player[]> GetAll();
        Task<Player> Create(NewPlayer newPlayer);
        Task<Player> Modify(Guid id, ModifiedPlayer modifiedPlayer);
        Task<Player> Delete(Guid id);

        Task<Item> GetItem(Guid playerId, Guid itemId);
        Task<Item[]> GetAllItems(Guid playerId);
        Task<Item> CreateItem(Guid playerId, NewItem item);
        Task<Item> DeleteItem(Guid playerId, Guid itemId);

    }

}