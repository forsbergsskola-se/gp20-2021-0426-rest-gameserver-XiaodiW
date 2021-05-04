namespace GitHubExplorer.Menu {

    public class OrgsMenu : Menu {
        public OrgsMenu(IResponseDate[] data) : base(data) {
            Data = data;
            Derived();
        }

        private void Derived() {
            RecentLevelTitle = "Org";
            NextLevelTitle = "members";
            NextUrl = new []{"https://api.github.com/orgs/","/members?page=1&per_page=1000"};
            NextChooseIndex = 3;
        }
    }

}