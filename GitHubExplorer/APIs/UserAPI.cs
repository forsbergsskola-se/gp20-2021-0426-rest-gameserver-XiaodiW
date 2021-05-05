using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using GitHubExplorer.Interfaces;

namespace GitHubExplorer.APIs {

    public class UserAPI: IUser {
        public async Task<IRepository> GetRepository(string repoName) {
            var url = new Uri($"https://api.github.com/repos/{login}/{repoName}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<RepositoryAPI>(responseData);
        }

        public async Task<List<RepositoryAPI>?> GetAllRepositories() {
            var url = new Uri($"https://api.github.com/users/{login}/repos?page=1&per_page=1000");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<List<RepositoryAPI>>(responseData);
        }
        
        public async Task<List<OrgnizationAPI>?> GetAllOrganizations() {
            var url = new Uri($"https://api.github.com/users/{login}/orgs?page=1&per_page=1000");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<List<OrgnizationAPI>>(responseData);
        }
        
        public string login { get; set; }
        public int id { get; set; }
        public string node_id{ get; set; }
        public string avatar_url{ get; set; }
        public string gravatar_id{ get; set; }
        public string url{ get; set; }
        public string html_url{ get; set; }
        public string followers_url{ get; set; }
        public string following_url{ get; set; }
        public string gists_url{ get; set; }
        public string starred_url{ get; set; }
        public string subscriptions_url{ get; set; }
        public string organizations_url{ get; set; }
        public string repos_url{ get; set; }
        public string events_url{ get; set; }
        public string received_events_url{ get; set; }
        public string type{ get; set; }
        public bool site_admin{ get; set; }
        public string name{ get; set; }
        public string company{ get; set; }
        public string blog{ get; set; }
        public string location{ get; set; }
        public string email{ get; set; }
        // public bool hireable{ get; set; }
        public string bio{ get; set; }
        public int public_repos{ get; set; }
        public int public_gists{ get; set; }
        public int followers{ get; set; }
        public int following{ get; set; }
        public string created_at{ get; set; }
        public string updated_at{ get; set; }
    }

}