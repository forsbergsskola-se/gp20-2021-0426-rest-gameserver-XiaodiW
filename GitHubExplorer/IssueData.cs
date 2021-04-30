namespace GitHubExplorer {

    public class IssueData {
        public string url { get; set; }
        public string repository_url { get; set; }
        public string labels_url { get; set; }
        public string comments_url { get; set; }
        public string events_url { get; set; }

        public string html_url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public int number { get; set; }
        public string title { get; set; }
        public UserData user { get; set; }
        public string state { get; set; }
        public bool locked { get; set; }
        public int comments { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        // public string pull_request { get; set; }
        // public string body { get; set; }
        

        public override string ToString() {
            var _title = title;
            if(title.Length > 25)
                _title = string.Concat(title.Substring(0, 15), "...", title.Substring(title.Length - 8, 7));
            else _title = title.PadRight(25);
            return $"{_title} {url}";
        }
    }

}