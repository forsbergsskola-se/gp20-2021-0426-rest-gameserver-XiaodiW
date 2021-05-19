using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Comm;
using Newtonsoft.Json;
using UnityEngine;

namespace Types.Player {

    public class Player
    {

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int Score { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public bool IsDeleted { get; set; }
        
        public DateTime CreationTime { get; private set; }
        public List<Item.Item> Items { get; set; }
        
        public List<string> Tag { get; set; }
        public List<Quest.Quest> Quests { get; set; }
        public DateTime LastGetQuests { get; set; }
        
        public static async Task<Player> GetPlayer(Guid id) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/delete");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            var result = JsonConvert.DeserializeObject<Player>(responseData);
            return result;
        }
        public static async Task<Player> CreatePlayer(string name) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players");
            var restPost = new RestApiPost(url,new NewPlayer(){Name = name});
            var responseData = await restPost.Post();
            Player newPlayer = null;
            try {
                newPlayer = JsonConvert.DeserializeObject<Player>(responseData, new JsonSerializerSettings());
            }
            catch(JsonException e) {
                Debug.Log(e.Message);
            }
            return newPlayer;
        }
    }

}