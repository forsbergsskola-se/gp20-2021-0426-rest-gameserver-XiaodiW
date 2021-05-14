using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MMORPGClient.APIs {

    public class Player {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }
        public List<Item> Items { get; set; }
        public List<string> Tag { get; set; }

        public static async Task<Player> CreatePlayer(string name) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players");
            var restPost = new RestApiPost(url,new NewPlayer(){Name = name});
            var responseData = await restPost.Post();
            var newPlayer = JsonConvert.DeserializeObject<Player>(responseData);
            return newPlayer;
        }
        public static async Task<Player> DeletePlayer(Guid id) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/delete");
            var restPost = new RestApiPost(url,null);
            var responseData = await restPost.Post();
            var newPlayer = JsonConvert.DeserializeObject<Player>(responseData);
            return newPlayer;
        }
        public static async Task<Player[]> ListAllPlayers() {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            var newPlayer = JsonConvert.DeserializeObject<List<Player>>(responseData)?.ToArray();
            return newPlayer;
        }
        
        public static async Task<Player[]> GetPlayerByName(string name) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players?playerName={name}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            var newPlayer = JsonConvert.DeserializeObject<List<Player>>(responseData)?.ToArray();
            return newPlayer;
        }
    }

}