using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitHubExplorer.Data;
using GitHubExplorer.Security;

namespace GitHubExplorer {

    internal class Program {
        private static string UserName;
        private static string Token;
        private static IResponseDate selectedData;
        private static IssueData _issue;

        private static async Task Main(string[] args) {
            Token = MicroSoftSecretsManager.LoadSecret("github-token");
            var exit = false;
            var chosenIndex =0;
            var url = new Uri("https://api.github.com/user");
            while(!exit) {
                var responseData = chosenIndex == 6 
                    ? await RestApiCommunication(url,_issue) 
                    : await RestApiCommunication(url);
                ResponseDataHandler(ref chosenIndex, responseData, ref url, ref exit);
            }
        }

        private static async Task<string> RestApiCommunication(Uri url) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Github Explorer");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", Token);
            var requestMeg = new HttpRequestMessage {RequestUri = url};
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
        
        private static async Task<string> RestApiCommunication(Uri url, IssueData issue) {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("Github Explorer");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", Token);
            var requestMeg = new HttpRequestMessage {RequestUri = url};
            requestMeg.Method = HttpMethod.Post;
            requestMeg.Content = new StringContent(JsonSerializer.Serialize(issue));
            Console.WriteLine(requestMeg.ToString());
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


        private static void ResponseDataHandler(ref int chosenIndex, string data, ref Uri url, ref bool exit) {
            switch(chosenIndex) {
                case 0:
                    var userInfo = JsonSerializer.Deserialize<UserData>(data);
                    UserName = userInfo.login;
                    Console.WriteLine(userInfo);
                    MainMenu(ref url, ref chosenIndex, ref exit);
                    break;
                case 1:
                    var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Console.WriteLine($"{UserName}'s Repositories");
                    for(var i = 0; i < reposInfo.Length; i++) Console.WriteLine($"{i}: {reposInfo[i]}");
                    ReposMenu(reposInfo,ref url, ref chosenIndex, ref exit);
                    break;
                case 2:
                    var orgsInfo = JsonSerializer.Deserialize<OrgsData[]>(data);
                    Console.WriteLine($"{UserName}'s Organizations");
                    for(var i = 0; i < orgsInfo.Length; i++) Console.WriteLine($"{i}: {orgsInfo[i]}");
                    OrgsMenu(orgsInfo, ref url, ref chosenIndex, ref exit);
                    break;
                case 3:
                    var memberInfo = JsonSerializer.Deserialize<UserData[]>(data);
                    Console.WriteLine($"{selectedData.GetName()}'s Members:");
                    for(var i = 0; i < memberInfo.Length; i++)
                        Console.WriteLine(
                            $"{string.Concat(i, ':').PadRight(3)}{memberInfo[i].login.PadRight(20)}({memberInfo[i].url})");
                    OrgsMemberMenu(memberInfo, ref url, ref chosenIndex, ref exit);
                    break;
                case 4:
                    var memberRepo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Console.WriteLine($"{selectedData.GetName()}'s Repositories");
                    for(var i = 0; i < memberRepo.Length; i++) Console.WriteLine($"{i}: {memberRepo[i]}");
                    MainMenu(ref url, ref chosenIndex, ref exit);
                    break;
                case 5:
                    var issuesInfo = JsonSerializer.Deserialize<IssueData[]>(data);
                    Console.WriteLine($"Repository {selectedData.GetName()}'s Issues");
                    for(var i = 0; i < issuesInfo.Length; i++) Console.WriteLine($"{i}: {issuesInfo[i]}");
                    MainMenu(ref url, ref chosenIndex, ref exit);
                    break;
            }
        }

        private static void MainMenu(ref Uri url, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" + "0: My Profile\n\r" +
                              "1: My Repositories\n\r" + "2: My Organizations\n\r" + "q: Exit\n\r");
            var r = Console.ReadLine();
            switch(r) {
                case "0":
                    url = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "1":
                    url = new Uri("https://api.github.com/user/repos");
                    chooseIndex = 1;
                    break;
                case "2":
                    url = new Uri("https://api.github.com/user/orgs");
                    chooseIndex = 2;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }

        private static void OrgsMenu(OrgsData[] data, ref Uri url, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" +
                              $"[0..{data.Length - 1}]: Goto see the Org's members \n\r" + "b:  Back to MainPage\n\r" +
                              "q:  Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            //Refresh the page if user typed number is out of boundary. 
            if(isNumber && linksIndex > data.Length - 1) s = "r";
            switch(s) {
                default:
                    if(isNumber)
                        url = new Uri(
                            $"https://api.github.com/orgs/{data[linksIndex].login}/members?page=1&per_page=1000");
                    chooseIndex = 3;
                    selectedData = data[linksIndex];
                    break;
                case "b":
                    url = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "r":
                    url = new Uri("https://api.github.com/user/orgs");
                    chooseIndex = 2;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }

        private static void OrgsMemberMenu(UserData[] data, ref Uri url, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" +
                              $"[0..{data.Length - 1}]: Goto see the member's repositories\n\r" +
                              "b:  Back to MainPage\n\r" + "q:  Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            //Refresh the page if user typed number is out of boundary. 
            if(isNumber && linksIndex > data.Length - 1) s = "b";
            switch(s) {
                default:
                    if(isNumber)
                        url = new Uri(
                            $"https://api.github.com/users/{data[linksIndex].login}/repos?page=1&per_page=1000");
                    chooseIndex = 4;
                    selectedData = data[linksIndex];
                    break;
                case "b":
                    url = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }
        
        private static void ReposMenu(ReposData[] data, ref Uri url, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" +
                              $"[0..{data.Length - 1}]: Goto see the Repo's Issues \n\r" + "b:  Back to MainPage\n\r" +
                              "q:  Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            //Refresh the page if user typed number is out of boundary. 
            if(isNumber && linksIndex > data.Length - 1) s = "r";
            switch(s) {
                default:
                    if(isNumber) {
                        var regex = new Regex("(.*?/issues)");
                        var issuesUrl = regex.Match(data[linksIndex].issues_url).Groups[0].Value;
                        url = new Uri(issuesUrl);
                        Console.WriteLine($"URL {url}");
                        selectedData = data[linksIndex];
                    }
                    chooseIndex = 5;
                    break;
                case "b":
                    url = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "r":
                    url = new Uri("https://api.github.com/user/repos");
                    chooseIndex = 2;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }
        private static void IssuesMenu(IssueData[] data, ref Uri url, ref int chooseIndex, ref bool exit) {
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" + "c:  create new issue\n\r" +
                              $"[0..{data.Length - 1}]: Goto see the Issue's detail \n\r" + "b:  Back to MainPage\n\r" +
                              "q:  Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            //Refresh the page if user typed number is out of boundary. 
            if(isNumber && linksIndex > data.Length - 1) s = "r";
            switch(s) {
                // default:
                //     if(isNumber) {
                //         url = new Uri(data[linksIndex].url);
                //         Console.WriteLine($"URL {url}");
                //         selectedData = data[linksIndex];
                //     }
                //     chooseIndex = 7;
                //     break;
                case "c":
                    _issue = CreateIssue();
                    chooseIndex = 6;
                    break;
                case "b":
                    url = new Uri("https://api.github.com/user");
                    chooseIndex = 0;
                    break;
                case "r":
                    url = new Uri("https://api.github.com/user/repos");
                    chooseIndex = 2;
                    break;
                case "q":
                    exit = true;
                    break;
            }
        }

        private static IssueData CreateIssue() {
            var issue = new IssueData();
            Console.WriteLine("Type in new issue' title:");
            issue.title = Console.ReadLine();
            return issue;
        }
    }

}