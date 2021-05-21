using System;
using System.Threading.Tasks;
using APIComm;
using Newtonsoft.Json;

namespace Types.Item {

    public class Item {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }

        public int Price { get; set; }
        public int LevelRequired { get; set; }

        public int LevelBonus { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEquipped { get; set; }

        public DateTime CreationTime { get; set; }
        
        [JsonIgnore]
        public Player.Player Player { get; set; }

        public static async Task<Item> HandleItem(Guid id, Guid itemId, ItemActions action) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/items/{itemId}/?action={action}");
            var restPost = new RestApiPost(url,null);
            var responseData = await restPost.Post();
            var result = JsonConvert.DeserializeObject<Item>(responseData);
            if(result.Id == Guid.Empty) throw new Exception(responseData);
            return result;
        }
    }

}