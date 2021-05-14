using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Filters;
using MMORPG.Types;

namespace MMORPG.Controllers {

    [ApiController]
    [Route("api/admin")]
    public class AdminController: ControllerBase {
        
        private readonly IRepository Repository;
        
        public AdminController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet("{id:guid}")] 
        public async Task<Player> Get(Guid id) {
            return await Repository.Get(id); 
        }
        [HttpGet]
        public async Task<Player[]> AdminGetAll() {
            return await Repository.AdminGetAll();
        }
        [HttpPost]
        public async Task<Player> Create(NewPlayer newPlayer) {
            return await Repository.Create(newPlayer);
        }
        [HttpPost("{id:guid}")]
        public async Task<Player> Modify(Guid id, ModifiedPlayer player) {
            return await Repository.Modify(id, player);
        }
        
        [HttpPost("{id:guid}/delete")]
        public async Task<Player> MarkDelete(Guid id) {
            return await Repository.MarkDelete(id);
        }
        [HttpDelete("{id:guid}")]
        public async Task<Player> Delete(Guid id) {
            return await Repository.Delete(id);
        }
        
        [HttpGet]
        [ActionName(nameof(GetScoreGt))]
        [ExactQueryParam("minScore")]
        public async Task<Player[]> GetScoreGt([FromQuery]int minScore) {
            return await Repository.GetScoreGt(minScore);
        }
        
        [HttpGet]
        [ActionName(nameof(GetScoreGt))]
        [ExactQueryParam("playerName")]
        public async Task<Player[]> GetPlayerByName([FromQuery]string playerName) {
            return await Repository.GetPlayerByName(playerName);
        }
        
    }

}