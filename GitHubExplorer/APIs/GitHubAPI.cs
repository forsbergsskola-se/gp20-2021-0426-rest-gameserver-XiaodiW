using System;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using GitHubExplorer.Interfaces;

namespace GitHubExplorer.APIs {

    public class GitHubAPI : IGitHubAPI {
        public async Task<IUser> GetUser(string userName) {
            var url = new Uri($"https://api.github.com/users/{userName}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<UserAPI>(responseData);
        }

        public async Task<IUser> GetUser() {
            var url = new Uri($"https://api.github.com/user");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<UserAPI>(responseData);
        }

        public async Task<IOrgnization> GetOrganization(string orgName) {
            var url = new Uri($"https://api.github.com/orgs/{orgName}");
            var restGet = new RestApiGet(url);
            var responseData = await restGet.Get();
            return JsonSerializer.Deserialize<OrgnizationAPI>(responseData);
        }
    }

}
    