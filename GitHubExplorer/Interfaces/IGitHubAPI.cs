using System.Threading.Tasks;

namespace GitHubExplorer.Interfaces {

    public interface IGitHubAPI {
        Task<IUser> GetUser(string userName);
        Task<IOrgnization> GetOrganization(string orgName);
    }

}