using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using GitHubExplorer.Interfaces;

namespace GitHubExplorer.APIs {

    public class OrgnizationAPI: IOrgnization {
        public async Task<IUser> GetMember(string memberName) {
            var url = new Uri($"https://api.github.com/orgs/{login}/{memberName}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<UserAPI>(responseData);
        }

        public async Task<List<UserAPI>?> GetAllMembers() {
            var url = new Uri($"https://api.github.com/orgs/{login}/members?page=1&per_page=1000");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<List<UserAPI>>(responseData);
        }

        public string login { get; set; }
        public int id { get; set; }
        public string node_id{ get; set; }
        public string url{ get; set; }
        public string repos_url{ get; set; }
        public string events_url{ get; set; }
        public string hooks_url{ get; set; }
        public string issues_url{ get; set; }
        public string members_url{ get; set; }
        public string public_members_url{ get; set; }
        public string avatar_url{ get; set; }
        public string gravatar_id{ get; set; }
        public string description{ get; set; }
    }

}