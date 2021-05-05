using System;
using System.Text.Json;
using System.Threading.Tasks;
using GitHubExplorer.APIs;
using GitHubExplorer.Comm;
using GitHubExplorer.Data;

namespace GitHubExplorer {

    internal class Program {
        public static string UserName;
        public static IResponseDate selectedData;

        private static async Task Main(string[] args) {
            if(args.Length == 0) {
                var chosenIndex = 0;
                var url = new Uri("https://api.github.com/user");
                while(chosenIndex >= 0) {
                    string responseData;
                    if(chosenIndex == 6) {
                        var restPost = new RestApiPost(url, CreateIssue());
                        responseData = await restPost.Post();
                        goto response;
                    }
                    var restGet = new RestApiGet(url);
                    responseData = await restGet.Get();
                    response :
                    ResponseDataHandler(ref chosenIndex, responseData, ref url);
                }
            } else { await RestAPI(); }
        }


        private static void ResponseDataHandler(ref int chosenIndex, string data, ref Uri url) {
            switch(chosenIndex) {
                case 0:
                    var userInfo = JsonSerializer.Deserialize<UserData>(data);
                    UserName = userInfo.login;
                    Menu.Menu.MainMenu(userInfo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 1:
                    var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Menu.Menu.RepoMenu(reposInfo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 2:
                    var orgsInfo = JsonSerializer.Deserialize<OrgsData[]>(data);
                    Menu.Menu.OrgsMenu(orgsInfo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 3:
                    var memberInfo = JsonSerializer.Deserialize<UserData[]>(data);
                    Menu.Menu.OrgsMemberMenu(memberInfo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 4:
                    var memberRepo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Menu.Menu.MemberReposMenu(memberRepo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 5:
                    var issuesInfo = JsonSerializer.Deserialize<IssueData[]>(data);
                    Menu.Menu.ReposIssuesMenu(issuesInfo, ref url, ref chosenIndex, ref selectedData);
                    break;
                case 6:
                    url = new Uri(selectedData.GetUrl() + "/issues");
                    chosenIndex = 5;
                    break;
                case 7:
                    Console.WriteLine(data);
                    var comments = JsonSerializer.Deserialize<CommentData[]>(data);
                    Menu.Menu.IssueCommentsMenu(comments, ref url, ref chosenIndex, ref selectedData);
                    break;
            }
        }

        private static IssueData CreateIssue() {
            var issue = new IssueData();
            Console.WriteLine("Type in new issue' title:");
            issue.title = Console.ReadLine();
            return issue;
        }

        private static async Task RestAPI() {
            Console.WriteLine("Type in userName");
            var input = Console.ReadLine();
            var gitHubAPI = new GitHubAPI();
            var user = await gitHubAPI.GetUser(input);
            Console.WriteLine($"User Name: {user.login}");
            var allOrganizations = await user.GetAllOrganizations();
            foreach(var orgnization in allOrganizations) { Console.WriteLine(orgnization.login); }
            var members = await allOrganizations[0].GetAllMembers();
            foreach(var member in members) { Console.WriteLine(member.login); }
        }
    }

}

