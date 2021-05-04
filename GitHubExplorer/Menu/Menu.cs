using System;

namespace GitHubExplorer.Menu {

    public class Menu {
        protected IResponseDate[] Data;
        public Uri Url { get; private set; }
        public int ChooseIndex { get; private set; }
        public bool Exit { get; private set; }
        public IResponseDate SelectedData { get; private set; }
        
        private protected string RecentLevelTitle;
        private protected string NextLevelTitle;
        private protected string[] NextUrl = new string[2];
        private protected int NextChooseIndex;



        protected Menu(IResponseDate[] data){
            Data = data;
        }
        
        public void DoMenu(ref Uri url, ref int chooseIndex, ref bool exit) {
            var options = NextLevelTitle != string.Empty
                ? $"[0..{Data.Length - 1}]: Goto see the {RecentLevelTitle}'s {NextLevelTitle}\n\r"
                : string.Empty;
            Console.WriteLine("\n\rWhat would you like to see next?\n\r" + 
                              $"{options}" + 
                              "p: My Profile\n\r" +
                              "r: My Repositories\n\r" + 
                              "o: My Organizations\n\r" + 
                              "q: Exit\n\r");
            var s = Console.ReadLine();
            var isNumber = int.TryParse(s, out var linksIndex);
            if(isNumber && linksIndex > Data.Length - 1) s = "b";
            switch(s) {
                default:
                    if(isNumber &&  NextLevelTitle != string.Empty) {
                        Url = new Uri(NextUrl[0]+Data[linksIndex].GetName()+NextUrl[1]);
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
                    Exit = true;
                    break;
            }
            url = Url;
            chooseIndex = ChooseIndex;
            exit = Exit;
        }
    }

}