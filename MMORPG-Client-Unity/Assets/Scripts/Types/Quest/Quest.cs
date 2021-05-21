using System;
using System.Threading.Tasks;
using APIComm;
using Newtonsoft.Json;

namespace Types.Quest {

    public class Quest {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public Item.Item Item { get; set; }
        public int Gold { get; set; }
        public bool RewardGold { get; set; }
        public DateTime CreateTime { get; set; }
        public int ExpiredTime { get; set; }
        public QuestStatus Status { get; set; }


        public static async Task<Player.Player> GetQuests(Guid id) {

            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/quest");
            var restPost = new RestApiPost(url, null);
            var responseData = await restPost.Post();
            var result = JsonConvert.DeserializeObject<Player.Player>(responseData);
            return result;
        }
        
        public static async Task<Player.Player> DoQuests(Guid id,Guid questId) {

            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/quest/{questId}");
            var restPost = new RestApiPost(url, null);
            var responseData = await restPost.Post();
            var result = JsonConvert.DeserializeObject<Player.Player>(responseData);
            if(result.Id == Guid.Empty) throw new Exception(responseData);
            return result;
        }
    }

}