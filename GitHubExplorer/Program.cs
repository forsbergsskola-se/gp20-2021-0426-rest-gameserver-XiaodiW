using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GitHubExplorer
{
    class Program {
        private static readonly HttpClient Client = new ();
        static async Task Main(string[] args) {
            var userName = LoadSecret("github-user");
            var token = LoadSecret("github-token");
            Console.WriteLine($"User: {userName}; Token: {token}");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            Client.DefaultRequestHeaders.UserAgent.TryParseAdd(userName);
            var requestMeg = new HttpRequestMessage();
            requestMeg.RequestUri = new Uri($"https://api.github.com/users/{userName}");
            requestMeg.Method = HttpMethod.Get;
            requestMeg.Headers.Authorization = AuthenticationHeaderValue.Parse(token);
            var received = Client.SendAsync(requestMeg);
            // Console.WriteLine(await received);
            Console.WriteLine(received.Result.StatusCode.ToString());
            var stream = await received.Result.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            var userInfo = JsonSerializer.Deserialize<UserResponse>(data);
            Console.WriteLine(userInfo);
            Client.Dispose();
        }
            /// <summary>
            /// Returns the saved secrets in Microsoft Secrets Manager.
            /// </summary>
            /// <param name="i">0:github-user; 1:github-token</param>
            /// <returns></returns>
        private static string LoadSecret(string key) {
            //preparation:
            //1. dotnet add package Microsoft.Extensions.Configuration.UserSecrets
            //   or add the package via NuGet.
            //2. dotnet user-secret init
            //3. dotnet use-secrets set "github-token" "MytokenSecrets"
            //  dotnet user-secrets list
            // dotnet user-secrets clear.
            // ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
            // %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var secretProvider = config.Providers.First();
            secretProvider.TryGet(key, out var token);
            return token;
        }

    }

}
