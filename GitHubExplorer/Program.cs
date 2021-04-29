using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubExplorer
{
    class Program {
        private static readonly HttpClient Client = new ();
        static readonly string _userName = "XiaodiW";
        static readonly string _token = "ghp_1QTag";
        static async Task Main(string[] args){
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            Client.DefaultRequestHeaders.UserAgent.TryParseAdd(_userName);
            var requestMeg = new HttpRequestMessage();
            requestMeg.RequestUri = new Uri($"https://api.github.com/users/{_userName}");
            requestMeg.Method = HttpMethod.Get;
            requestMeg.Headers.Authorization = AuthenticationHeaderValue.Parse(_token);
            var received = Client.SendAsync(requestMeg);
            Console.WriteLine(await received);
            var stream = await received.Result.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            var userInfo = JsonSerializer.Deserialize<UserResponse>(data);
            Console.WriteLine($"Name: {userInfo.name}");
            Console.WriteLine($"Link: {userInfo.html_url}");
            Console.WriteLine($"Repos: {userInfo.repos_url}");
            Console.WriteLine($"Created at: {userInfo.created_at}");
            Client.Dispose();
        }


    }

    public class UserResponse {
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
        public string hireable{ get; set; }
        public string bio{ get; set; }
        public int public_repos{ get; set; }
        public int public_gists{ get; set; }
        public int followers{ get; set; }
        public int following{ get; set; }
        public string created_at{ get; set; }
        public string updated_at{ get; set; }
    }

    /*public class Secrets {
        public string Token { get; set; }
        private static readonly string FileName = Path.Combine(Environment.CurrentDirectory, "secrets.json");

        public Secrets(string token) {
            Token = token;
        }

        private static Secrets LoadAndValidateSectets() {
            Secrets secrets;
            if(!File.Exists(FileName)) {
                var token = Console.ReadLine();
                secrets = new Secrets(token);
                File.WriteAllText(FileName, JsonSerializer.Serialize(secrets));
            } else { secrets = JsonSerializer.Deserialize<Secrets>(File.ReadAllText(FileName)); }
            if(string.IsNullOrEmpty(secrets.Token)) {
                Console.WriteLine("Error:");
                throw new Exception($"Error: Need to define a Token in file {FileName}.");
            }
            return secrets;
        }
    }*/
}
