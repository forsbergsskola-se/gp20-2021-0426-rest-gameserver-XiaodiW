using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Filters;
using MMORPG.Types;
using MMORPG.Types.Player;

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
        
        [HttpPost("{id:guid}/delete")]
        public async Task<Player> MarkDelete(Guid id) {
            return await Repository.MarkDelete(id);
        }
        [HttpDelete("{id:guid}")]
        public async Task<Player> Delete(Guid id) {
            return await Repository.Delete(id);
        }
        
        [HttpPost("{id:guid}/quest")]
        public async Task<Player> GetQuests(Guid id) {
            return await Repository.GetQuests(id);
        }
        
        [HttpPost("{id:guid}/quest/{questId:guid}")]
        public async Task<Player> DoQuests(Guid id,Guid questId) {
            return await Repository.DoQuests(id,questId);
        }
        
        [HttpPost("{id:guid}/level")]
        [ExactQueryParam("byGold")]
        public async Task<Player> UpgradeLevel(Guid id,[FromQuery]int gold) {
            return await Repository.UpgradeLevel(id,gold);
        }
        
        [HttpPost("{id:guid}/level")]
        public async Task<Player> UpgradeLevel(Guid id) {
            return await Repository.UpgradeLevel(id);
        }
        
        [HttpGet]
        [ActionName(nameof(GetScoreGt))]
        [ExactQueryParam("minScore")]
        public async Task<Player[]> GetScoreGt([FromQuery]int minScore) {
            return await Repository.GetScoreGt(minScore);
        }
        
        [HttpGet]
        [ActionName(nameof(GetPlayerByName))]
        [ExactQueryParam("playerName")]
        public async Task<Player[]> GetPlayerByName([FromQuery]string playerName) {
            return await Repository.GetPlayerByName(playerName);
        }
        
        [HttpGet]
        [ActionName(nameof(GetLeaderBoard))]
        [ExactQueryParam("orderBy")]
        public async Task<Player[]> GetLeaderBoard([FromQuery]LeaderBoardOrderBy orderBy) {
            return await Repository.GetLeaderBoard(orderBy);
        }
    }

}