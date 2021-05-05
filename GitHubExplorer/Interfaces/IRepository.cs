using System.Collections.Generic;

namespace GitHubExplorer.Interfaces {

    public interface IRepository {
        List<IIssue> GetIssues();
        IIssue GetAllIssues(string issueName);
        string name { get; }
        string description { get; }
        
    }

}