namespace GitHubExplorer.Data {

    public class OrgsData: IResponseDate {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id{ get; set; }
        public string url{ get; set; }
        public string repos_url{ get; set; }
        public string events_url{ get; set; }
        public string hooks_url{ get; set; }
        public string issues_url{ get; set; }
        public string members_url{ get; set; }
        public string public_members_url{ get; set; }
        public string avatar_url{ get; set; }
        public string gravatar_id{ get; set; }
        public string description{ get; set; }

        public override string ToString() {
            return $"{login.PadRight(18)}" +
                   $"({url})" + 
                   $"{description.Substring(0,80)}";
        }

        public string GetName() {
            return login;
        }

        public string GetUrl() {
            return url;
        }
    }

}