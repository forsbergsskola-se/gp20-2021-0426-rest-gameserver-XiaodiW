using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Types;

namespace MMORPG.Controllers {

    [ApiController]
    [Route("api/players")]

    public class ItemsController: ControllerBase {
        
        private readonly IRepository Repository;
        
        public ItemsController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet("{playerId:guid}/{itemId:guid}")] 
        public async Task<Item> GetItem(Guid playerId, Guid itemId) {
            return await Repository.GetItem(playerId,itemId);
        }
        [HttpGet("{playerId:guid}/items")]
        public async Task<Item[]> GetAllItems(Guid playerId) {
            return await Repository.GetAllItems(playerId);
        }
        [HttpPost("{playerId:guid}/items")]
        public async Task<Item> CreateItem(Guid playerId, NewItem item) {
            return await Repository.AddItem(playerId,item);
        }
        [HttpDelete("{playerId:guid}/items")]
        public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
            return await Repository.DeleteItem(playerId,itemId);
        }
    }

}