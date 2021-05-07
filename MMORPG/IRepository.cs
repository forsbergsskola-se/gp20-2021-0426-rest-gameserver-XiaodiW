using System;
using System.Threading.Tasks;

namespace MMORPG {

    public interface IRepository
    {
        Task<Player> Get(Guid id);
        Task<Player[]> GetAll();
        Task<Player> Create(string name);
        Task<Player> Modify(Guid id, Player player);
        Task<Player> Delete(Guid id);
    }

}