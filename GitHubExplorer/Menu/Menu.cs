using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GitHubExplorer.Data;

namespace GitHubExplorer.Menu {

    public static class Menu {
        private static IResponseDate[] Data;
        private static Uri Url;
        private static int ChooseIndex;
        private static IResponseDate SelectedData;

        private static void DoMenu(Dictionary<string, MenuElement> elements, Func<IResponseDate, Uri> UrlMethod) {
            Console.WriteLine("********************************");
            Console.WriteLine("What would you like to see next?");
            foreach(var element in elements) {
                var prefix = element.Key == "n" ? string.Empty : element.Key + ": ";
                Console.WriteLine($"{prefix}{element.Value.title}" );
            }
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            if(isNumber) {
                s = "n";
                if(linksIndex > Data.Length - 1 || linksIndex < 0) s = "p";
            }
            if(elements.ContainsKey(s)) {
                Url = elements[s].url;
                ChooseIndex = elements[s].chooseIndex;
                if(s == "n") {
                    Url = UrlMethod(Data[linksIndex]);
                    SelectedData = Data[linksIndex];
                }
            }
        
        }

        public static void MainMenu(IResponseDate data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Console.WriteLine(data);
            var usersInfo = new[] {data};
            Data = usersInfo;
            DoMenu(new MenuElement().DefaultElements(),UrlEmputy);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }

        private static Uri UrlEmputy(IResponseDate data) {
            return new("");
        }

        public static void OrgsMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Data = data;
            Console.WriteLine($"{Program.UserName}'s Organizations");
            for(var i = 0; i < data.Length; i++) Console.WriteLine($"{i}: {data[i]}");
            var elements = new MenuElement().DefaultElements();
            elements.Add("n", new MenuElement("n",$"[0..{Data.Length - 1}]: Select a Organization and see its members",null,3));
            DoMenu(elements,UrlOrgsMembers);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }

        private static Uri UrlOrgsMembers(IResponseDate data) {
            var orgData = (OrgsData) data;
            return new Uri($"https://api.github.com/orgs/{orgData.login}/members?page=1&per_page=1000");
        }

        public static void RepoMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Console.WriteLine($"{Program.UserName}'s Repositories");
            for(var i = 0; i < data.Length; i++) Console.WriteLine($"{i}: {data[i]}");
            Data = data;
            var elements = new MenuElement().DefaultElements();
            elements.Add("n", new MenuElement("n",$"[0..{Data.Length - 1}]: Select a Repository and see its Issues",null,5));
            DoMenu(elements,UrlReposIssue);
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

        public static void OrgsMemberMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Data = data;
            Console.WriteLine($"{Program.selectedData.GetName()}'s Members:");
            for(var i = 0; i < data.Length; i++)
                Console.WriteLine(
                    $"{string.Concat(i, ':').PadRight(3)}{data[i].GetName().PadRight(20)}({data[i].GetUrl()})");
            var elements = new MenuElement().DefaultElements();
            elements.Add("n", new MenuElement("n",$"[0..{Data.Length - 1}]: Select a Member and see its Repositories",null,4));
            DoMenu(elements,UrlMemberRepos);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }

        private static Uri UrlMemberRepos(IResponseDate data) {
            return new($"https://api.github.com/users/{data.GetName()}/repos?page=1&per_page=1000");
        }

        public static void MemberReposMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Data = data;
            Console.WriteLine($"{Program.selectedData.GetName()}'s Repositories");
            for(var i = 0; i < data.Length; i++) Console.WriteLine($"{i}: {data[i]}");
            DoMenu(new MenuElement().DefaultElements(),UrlEmputy);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }

        public static void ReposIssuesMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Data = data;
            Console.WriteLine($"Repository {Program.selectedData.GetName()}'s Issues");
            for(var i = 0; i < data.Length; i++) Console.WriteLine($"{i}: {data[i]}");
            var elements = new MenuElement().DefaultElements();
            elements.Add("c", new MenuElement("c",$"Create a new Issue in Repository of {Program.selectedData.GetName()}",url,6));
            elements.Add("n", new MenuElement("n",$"[0..{Data.Length - 1}]: Select a Issue and see its information",null,7));
            DoMenu(elements,UrlIssueDetail);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }
        
        private static Uri UrlIssueDetail(IResponseDate data) {
            return new($"{data.GetUrl()}");
        }
        
        public static void IssuesDetailMenu(IResponseDate[] data, ref Uri url, ref int chooseIndex,
            ref IResponseDate selectedData) {
            Data = data;
            Console.WriteLine($"Issues {Program.selectedData.GetName()}'s information");
            for(var i = 0; i < data.Length; i++) Console.WriteLine($"{i}: {data[i]}");
            var elements = new MenuElement().DefaultElements();
            DoMenu(elements,UrlIssueDetail);
            url = Url;
            chooseIndex = ChooseIndex;
            selectedData = SelectedData;
        }
    }

}