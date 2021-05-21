using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using APIComm;
using Handler;
using Newtonsoft.Json;
using UnityEngine;

namespace Types.Player {

    public class Player
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public bool IsDeleted { get; set; }
        
        public DateTime CreationTime { get; set; }
        public List<Item.Item> Items { get; set; }
        
        public List<string> Tag { get; set; }
        public List<Quest.Quest> Quests { get; set; }
        public DateTime LastGetQuests { get; set; }
        
        public static async Task<Player> GetPlayer(Guid id) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            var result = JsonConvert.DeserializeObject<Player>(responseData);
            return result;
        }
        public static async Task<Player> CreatePlayer(string name) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players");
            var restPost = new RestApiPost(url,new NewPlayer(){Name = name});
            var responseData = await restPost.Post();
            var  result = JsonConvert.DeserializeObject<Player>(responseData);
            return result;
        }
        
        public static async Task<Player> UpgradeLevel(Guid id){
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/level");
            var restPost = new RestApiPost(url,null);
            var responseData = await restPost.Post();
            var  result = JsonConvert.DeserializeObject<Player>(responseData);
            if(result.Id == Guid.Empty) throw new Exception(responseData);
            return result;
        }
        
        public static async Task<Player[]> GetLeaderBoard(LeaderBoardOrderBy orderBy) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players?orderBy={orderBy}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            var result = JsonConvert.DeserializeObject<Player[]>(responseData);
            return result;
        }
    }

}