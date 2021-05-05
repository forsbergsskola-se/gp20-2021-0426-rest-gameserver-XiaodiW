using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using GitHubExplorer.APIs;

namespace GitHubExplorer.Interfaces {

    public interface IOrgnization {
        Task<IUser> GetMember(string memberName);
        Task<List<UserAPI>> GetAllMembers();
        string login { get; }
        string description { get; }
    }

}