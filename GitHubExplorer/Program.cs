using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GitHubExplorer.Comm;
using GitHubExplorer.Data;
using GitHubExplorer.Security;

namespace GitHubExplorer {

    internal class Program {
        public static string UserName;
        public static IResponseDate selectedData;
        private static async Task Main(string[] args) {
            var chosenIndex =0;
            var url = new Uri("https://api.github.com/user");
            while(chosenIndex >=0) {
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
        }
        
        private static void ResponseDataHandler(ref int chosenIndex, string data, ref Uri url) {
            switch(chosenIndex) {
                case 0:
                    var userInfo = JsonSerializer.Deserialize<UserData>(data);
                    UserName = userInfo.login;
                    Menu.Menu.MainMenu(userInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 1:
                    var reposInfo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Menu.Menu.RepoMenu(reposInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 2:
                    var orgsInfo = JsonSerializer.Deserialize<OrgsData[]>(data);
                    Menu.Menu.OrgsMenu(orgsInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 3:
                    var memberInfo = JsonSerializer.Deserialize<UserData[]>(data);
                    Menu.Menu.OrgsMemberMenu(memberInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 4:
                    var memberRepo = JsonSerializer.Deserialize<ReposData[]>(data);
                    Menu.Menu.MemberReposMenu(memberRepo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 5:
                    var issuesInfo = JsonSerializer.Deserialize<IssueData[]>(data);
                    Menu.Menu.ReposIssuesMenu(issuesInfo,ref url,ref chosenIndex,ref selectedData);
                    break;
                case 6:
                    url = new Uri(selectedData.GetUrl()+ "/issues");
                    chosenIndex = 5;
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