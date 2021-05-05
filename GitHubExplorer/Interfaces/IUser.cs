using System.Collections.Generic;
using System.Threading.Tasks;
using GitHubExplorer.APIs;

namespace GitHubExplorer.Interfaces {

    public interface IUser {
        Task<IRepository> GetRepository(string repoName);
        Task<List<RepositoryAPI>> GetAllRepositories();
        
        Task<List<OrgnizationAPI>> GetAllOrganizations();
        string login { get; }
        string location { get; }
    }

}