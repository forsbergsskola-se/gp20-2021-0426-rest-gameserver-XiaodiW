using System.Collections.Generic;

namespace GitHubExplorer.Interfaces {

    public interface IIssue {
        IComment GetComment(string commentName);
        List<IComment> GetAllComments();
        string Name { get; }
        string Description { get; }
    }

}