namespace GitHubExplorer.Data {

    public class CommentData: IResponseDate {
        public string url { get; set; }
        public string html_url { get; set; }
        public string issue_url { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public UserData user { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string author_association { get; set; }
        public string body { get; set; }
        public string performed_via_github_app  { get; set; }

        public override string ToString() {
            var _title = body;
            if(body.Length > 25)
                _title = string.Concat(body.Substring(0, 15), "...", body.Substring(body.Length - 8, 7));
            else _title = body.PadRight(25);
            return $"{_title} {url}";
        }
        public string GetName() {
            return body;
        }

        public string GetUrl() {
            return url;
        }
    }

}