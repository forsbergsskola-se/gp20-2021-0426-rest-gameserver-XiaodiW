using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MMORPG {
    [ApiController]
    [Route("api/player")]

    public class PlayersController: ControllerBase {
        
        private readonly IRepository Repository;
        
        public PlayersController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet] 
        public async Task<Player> Get(Guid id) {
            return await Repository.Get(id);
        }
        [HttpGet("all")]
        public async Task<Player[]> GetAll() {
            return await Repository.GetAll();
        }
        [HttpPost("new")]
        public async Task<Player> Create(string name) {
            return await Repository.Create(name);
        }
        [HttpPost]
        public async Task<Player> Modify(Guid id, Player player) {
            return await Repository.Modify(id, player);
        }
        [HttpDelete]
        public async Task<Player> Delete(Guid id) {
            return await Repository.Delete(id);
        }
    }

}