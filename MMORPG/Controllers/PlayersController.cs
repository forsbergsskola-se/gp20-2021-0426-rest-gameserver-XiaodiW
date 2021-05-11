using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Types;

namespace MMORPG.Controllers {
    [ApiController]
    [Route("api/players")]

    public class PlayersController: ControllerBase {
        
        private readonly IRepository Repository;
        
        public PlayersController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet("{id:guid}")] 
        public async Task<Player> Get(Guid id) {
            return await Repository.Get(id); 
        }
        [HttpGet]
        public async Task<Player[]> GetAll() {
            return await Repository.GetAll();
        }
        [HttpPost]
        public async Task<Player> Create(NewPlayer newPlayer) {
            return await Repository.Create(newPlayer);
        }
        [HttpPost("{id:guid}")]
        public async Task<Player> Modify(Guid id, ModifiedPlayer player) {
            return await Repository.Modify(id, player);
        }
        [HttpDelete("{id:guid}")]
        public async Task<Player> Delete(Guid id) {
            return await Repository.Delete(id);
        }
    }

}