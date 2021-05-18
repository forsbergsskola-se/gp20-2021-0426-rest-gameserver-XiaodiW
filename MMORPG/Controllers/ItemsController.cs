using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Filters;
using MMORPG.Types;
using MMORPG.Types.Item;

namespace MMORPG.Controllers {

    [ApiController]
    [Route("api/players")]
    public class ItemsController: ControllerBase {
        
        private readonly IRepository Repository;
        
        public ItemsController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet("{playerId:guid}/items/{itemId:guid}")] 
        public async Task<Item> GetItem(Guid playerId, Guid itemId) {
            return await Repository.GetItem(playerId,itemId);
        }
        [HttpGet("{playerId:guid}/items")]
        public async Task<Item[]> GetAllItems(Guid playerId) {
            return await Repository.GetAllItems(playerId);
        }
        
        [HttpPost("{id:guid}/items/{itemId:guid}/{action}")] 
        public async Task<Item> SoldItem(Guid id, Guid itemId, ItemActions action) {
            return await Repository.HandleItem(id,itemId,action);
        }
        
        [HttpPost("{playerId:guid}/items")]
        public async Task<Item> CreateItem(Guid playerId, NewItem item) {
            return await Repository.AddItem(playerId,item);
        }
        [HttpDelete("{playerId:guid}/items/{itemId:guid}")]
        public async Task<Item> DeleteItem(Guid playerId, Guid itemId) {
            return await Repository.DeleteItem(playerId,itemId);
        }
        
        
    }

}