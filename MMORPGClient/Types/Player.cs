using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Comm;

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
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };
            var newPlayer = JsonSerializer.Deserialize<Player>(responseData,options);
            return newPlayer;
        }
        public static async Task<Player> DeletePlayer(Guid id) {
            var url = new Uri($"{GlobalSetting.UrlRoot}/players/{id}/delete");
            var restPost = new RestApiPost(url,null);
            var responseData = await restPost.Post();
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };
            var newPlayer = JsonSerializer.Deserialize<Player>(responseData,options);
            return newPlayer;
        }
    }

}