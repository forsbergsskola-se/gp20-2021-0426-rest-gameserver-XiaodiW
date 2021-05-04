namespace GitHubExplorer.Menu {

    public class MainMenu : Menu {
        public MainMenu(IResponseDate[] data) : base(data) {
            Data = data;
            Derived();
        }

        private void Derived() {
            RecentLevelTitle = string.Empty;
            NextLevelTitle = string.Empty;
            NextUrl = new []{string.Empty,string.Empty};
            NextChooseIndex = 0;
        }
    }

}