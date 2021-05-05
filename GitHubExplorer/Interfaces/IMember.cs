using System.Collections.Generic;

namespace GitHubExplorer.Interfaces {

    public interface IMember {
        IRepository GetRepository(string repoName);
        List<IRepository> GetAllRepositories();
        string Name { get; }
        string Location { get; }
    }

}