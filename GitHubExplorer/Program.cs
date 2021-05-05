using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using GitHubExplorer.Data;
using GitHubExplorer.Security;

namespace GitHubExplorer {

    internal class Program {
        private static string UserName;
        private static IResponseDate selectedData;
        private static IssueData _issue;

        private static async Task Main(string[] args) {
            var chosenIndex =0;
            var url = new Uri("https://api.github.com/user");
            while(chosenIndex >=0) {
                string responseData;
                if(chosenIndex == 6) {
                    var restPost = new RestApiPost(url, _issue);
                    responseData = await restPost.Post();
                    goto response;
                }
                var restGet = new RestApiGet(url);
                responseData = await restGet.Get();
                response :
                ResponseDataHandler(ref chosenIndex, responseData, ref url);
            }
        }
        
        private static void ResponseDataHandler(ref int chosenIndex, string data, ref Uri url) {
            switch(chosenIndex) {
                case 0:
                    var userInfo = JsonSerializer.Deserialize<UserData>(data);
                    UserName = userInfo.login;
                    Console.WriteLine(userInfo);
                    var usersInfo = new [] {userInfo};
                    Menu.MainMenu(usersInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 1:
                    var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Console.WriteLine($"{UserName}'s Repositories");
                    for(var i = 0; i < reposInfo.Length; i++) Console.WriteLine($"{i}: {reposInfo[i]}");
                    Menu.RepoMenu(reposInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 2:
                    var orgsInfo = JsonSerializer.Deserialize<OrgsData[]>(data);
                    Console.WriteLine($"{UserName}'s Organizations");
                    for(var i = 0; i < orgsInfo.Length; i++) Console.WriteLine($"{i}: {orgsInfo[i]}");
                    Menu.OrgsMenu(orgsInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 3:
                    var memberInfo = JsonSerializer.Deserialize<UserData[]>(data);
                    Console.WriteLine($"{selectedData.GetName()}'s Members:");
                    for(var i = 0; i < memberInfo.Length; i++)
                        Console.WriteLine(
                            $"{string.Concat(i, ':').PadRight(3)}{memberInfo[i].login.PadRight(20)}({memberInfo[i].url})");
                    Menu.OrgsMemberMenu(memberInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 4:
                    var memberRepo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Console.WriteLine($"{selectedData.GetName()}'s Repositories");
                    for(var i = 0; i < memberRepo.Length; i++) Console.WriteLine($"{i}: {memberRepo[i]}");
                    Menu.MainMenu(memberRepo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 5:
                    var issuesInfo = JsonSerializer.Deserialize<IssueData[]>(data);
                    Console.WriteLine($"Repository {selectedData.GetName()}'s Issues");
                    for(var i = 0; i < issuesInfo.Length; i++) Console.WriteLine($"{i}: {issuesInfo[i]}");
                    Menu.MainMenu(issuesInfo,ref url,ref chosenIndex,ref selectedData);
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