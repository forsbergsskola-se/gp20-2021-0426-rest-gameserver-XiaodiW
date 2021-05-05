using System;
using System.Text.RegularExpressions;
using GitHubExplorer.Data;

namespace GitHubExplorer {

    public static class Menu {
        private static string RecentLevelTitle;
        private static string NextLevelTitle;
        private static int NextChooseIndex;

        private static IResponseDate[] Data;
        private static Uri Url;
        private static int ChooseIndex;
        private static IResponseDate SelectedData;
        private static void DoMenu(Func<IResponseDate, Uri> UrlMethod) {
            var options = NextLevelTitle != string.Empty
                ? $"[0..{Data.Length - 1}]: Select a {RecentLevelTitle} and see its {NextLevelTitle}\n\r"
                : string.Empty;
            var str = $"[0..{Data.Length - 1}]:";
            var len = str.Length-2;
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" + 
                              $"{options}" + 
                              "p:"+ $"{new string(' ', len)}" + " My Profile\n\r" +
                              "r:"+ $"{new string(' ', len)}" + " My Repositories\n\r" + 
                              "o:"+ $"{new string(' ', len)}" + " My Organizations\n\r" + 
                              "q:"+ $"{new string(' ', len)}" + " Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            if(isNumber && linksIndex > Data.Length - 1) s = "p";
            switch(s) {
                default:
                    if(isNumber &&  NextLevelTitle != string.Empty) {
                        Url = UrlMethod(Data[linksIndex]);
                        ChooseIndex = NextChooseIndex;
                        SelectedData = Data[linksIndex];
                    }
                    break;
                case "p":
                    Url = new Uri("https://api.github.com/user");
                    ChooseIndex = 0;
                    break;
                case "r":
                    Url = new Uri("https://api.github.com/user/repos");
                    ChooseIndex = 1;
                    break;
                case "o":
                    Url = new Uri("https://api.github.com/user/orgs");
                    ChooseIndex = 2;
                    break;
                case "q":
                    ChooseIndex = -1;
                    break;
            }
        }
        
        public static void MainMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,ref IResponseDate selectedData) {
            RecentLevelTitle = string.Empty;
            NextLevelTitle = string.Empty;
            NextChooseIndex = 0;
            Data = data;
            DoMenu(UrlMain);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }

        private static Uri UrlMain(IResponseDate data) {
            return new Uri("");
        }
        
        public static void OrgsMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,ref IResponseDate selectedData) {
            RecentLevelTitle = "Org";
            NextLevelTitle = "members";
            NextChooseIndex = 3;
            Data = data;
            DoMenu(UrlOrgsMembers);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }
        
        private static Uri UrlOrgsMembers(IResponseDate data) {
            var orgData = (OrgsData) data;
            return new Uri($"https://api.github.com/orgs/{orgData.login}/members?page=1&per_page=1000");
        }
        
        public static void RepoMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,ref IResponseDate selectedData) {
            RecentLevelTitle = "Repo";
            NextLevelTitle = "issues";
            NextChooseIndex = 5;
            Data = data;
            DoMenu(UrlReposIssue);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }
        
        private static Uri UrlReposIssue(IResponseDate data) {
            var regex = new Regex("(.*?/issues)");
            var repoData = (ReposData) data;
            var issuesUrl = regex.Match(repoData.issues_url).Groups[0].Value;
            return new Uri(issuesUrl);
            
        }
        public static void OrgsMemberMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,ref IResponseDate selectedData) {
            RecentLevelTitle = "Member";
            NextLevelTitle = "Repositories";
            NextChooseIndex = 4;
            Data = data;
            DoMenu(UrlMemberRepos);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }
        
        private static Uri UrlMemberRepos(IResponseDate data) {
            return new Uri(
                $"https://api.github.com/users/{data.GetName()}/repos?page=1&per_page=1000");
        }
    }

}