using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GitHubExplorer {

    internal class Program {
        private static string UserName;
        private static string Token;

        private static async Task Main(string[] args) {
            UserName = LoadSecret("github-user");
            Token = LoadSecret("github-token");
            Console.WriteLine($"User: {UserName}; Token: {Token}");

            var exit = false;
            var chooseIndex = 0;
            var URL = new Uri($"https://api.github.com/users/{UserName}");
            while(!exit) {
                var data = await RestApiCommunication(URL);
                
                switch(chooseIndex) {
                    case 0:
                        var userInfo = JsonSerializer.Deserialize<UserResponse>(data);
                        Console.WriteLine(userInfo);
                        break;
                    case 1:
                        var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                        for(var i = 0; i < reposInfo.Length; i++)
                            Console.WriteLine($"{i.ToString().PadRight(3)}  {reposInfo[i]}");
                        break;
                }

                Console.WriteLine(
                    "\n\rWhat would you like to see next?\n\r" +
                    "0: User's Profile\n\r" + 
                    "1: Repositories\n\r" + 
                    "2: Organizations\n\r" + 
                    "b: Back to MainPage\n\r" + 
                    "q: Exit");
                var r = Console.ReadLine();
                switch(r) {
                    case "0":
                        URL = new Uri($"https://api.github.com/users/{UserName}");
                        chooseIndex = 0;
                        break;
                    case "1":
                        URL = new Uri($"https://api.github.com/users/{UserName}/repos");
                        chooseIndex = 1;
                        break;
                    case "2":
                        URL = new Uri($"https://api.github.com/users/{UserName}/orgs");
                        chooseIndex = 2;
                        break;
                    case "b":
                        URL = new Uri($"https://api.github.com/users/{UserName}");
                        chooseIndex = 0;
                        break;
                    case "q":
                        exit = true;
                        break;
                    default: 
                        URL = new Uri($"https://api.github.com/users/{UserName}");
                        chooseIndex = 0;
                        break;
                }
            }
        }

        private static async Task<string> RestApiCommunication(Uri URL) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd(UserName);
            var requestMeg = new HttpRequestMessage();
            requestMeg.RequestUri = URL;
            requestMeg.Method = HttpMethod.Get;
            requestMeg.Headers.Authorization = AuthenticationHeaderValue.Parse(Token);
            var received = client.SendAsync(requestMeg);
            // Console.WriteLine(await received);
            Console.WriteLine($"Server response status: {received.Result.StatusCode.ToString()}");
            var stream = await received.Result.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            client.Dispose();
            return data;
        }

        #region Microsoft Secret Manger

        /// <summary>
        ///     Returns the saved secrets in Microsoft Secrets Manager.
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

        #endregion
    }

}
