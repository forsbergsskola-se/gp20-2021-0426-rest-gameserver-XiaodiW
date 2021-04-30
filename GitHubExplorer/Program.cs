using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubExplorer {

    internal class Program {
        private static string UserName;
        private static string Token;

        private static async Task Main(string[] args) {
            Token = MicroSoftSecretsManager.LoadSecret("github-token");
            var exit = false;
            var chooseIndex = 0;
            var URL = new Uri($"https://api.github.com/user");
            while(!exit) {
                var data = await RestApiCommunication(URL);
                
                switch(chooseIndex) {
                    case 0:
                        var userInfo = JsonSerializer.Deserialize<UserData>(data);
                        UserName = userInfo.login;
                        Console.WriteLine(userInfo);
                        break;
                    case 1:
                        var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                        Console.WriteLine($"{UserName}'s Repositories");
                        for(var i = 0; i < reposInfo.Length; i++)
                            Console.WriteLine($"{i}: {reposInfo[i]}");
                        break;
                    case 2:
                        var orgsInfo = JsonSerializer.Deserialize<OrgsData[]>(data);
                        Console.WriteLine($"{UserName}'s Organizations");
                        for(var i = 0; i < orgsInfo.Length; i++)
                            Console.WriteLine($"{i}: {orgsInfo[i]}");
                        OrgsMenu(orgsInfo,ref URL, ref chooseIndex, ref exit);
                        goto NextLoop;
                    case 3:
                        var memberInfo = JsonSerializer.Deserialize<UserData[]>(data);
                        for(var i = 0; i < memberInfo.Length; i++) {
                            Console.WriteLine($"{String.Concat(i, ':').PadRight(3)}{memberInfo[i].login.PadRight(20)}({memberInfo[i].url})");
                        }
                        break;
                }
                MainMenu(ref URL, ref chooseIndex, ref exit);
                NextLoop: 
                ;
            }
        }

        private static void MainMenu(ref Uri URL, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" + "0: User's Profile\n\r" + "1: Repositories\n\r" +
                              "2: Organizations\n\r" + "b: Back to MainPage\n\r" + "q: Exit\n\r");
            var r = Console.ReadLine();
            switch(r) {
                case "0":
                    URL = new Uri($"https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "1":
                    URL = new Uri($"https://api.github.com/user/repos");
                    chooseIndex = 1;
                    break;
                case "2":
                    URL = new Uri($"https://api.github.com/user/orgs");
                    chooseIndex = 2;
                    break;
                case "b":
                    URL = new Uri($"https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }

        private static void OrgsMenu(OrgsData[] data, ref Uri URL, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" +
                              "line number(Goto see the repo's members) \n\r" + "b: Back to MainPage\n\r" + "q: Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            //Refresh the page if user typed number is out of boundary. 
            if(isNumber && linksIndex > data.Length - 1) s = "r";
            switch(s) {
                default:
                    if(isNumber) URL = new Uri($"https://api.github.com/orgs/{data[linksIndex].login}/members?page=1&per_page=1000");
                    chooseIndex = 3;
                    break;
                case "b":
                    URL = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "r":
                    URL = new Uri($"https://api.github.com/user/orgs");
                    chooseIndex = 2;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }

        private static async Task<string> RestApiCommunication(Uri URL) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Github Explorer");
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("token",Token);
            var requestMeg = new HttpRequestMessage {RequestUri = URL};
            // Console.WriteLine(requestMeg.ToString());
            var received = client.SendAsync(requestMeg);
            // Console.WriteLine(await received);
            // Console.WriteLine($"Server response status: {received.Result.StatusCode.ToString()}");
            var stream = await received.Result.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            // Console.WriteLine(data);
            client.Dispose();
            return data;
        }
    }

}
