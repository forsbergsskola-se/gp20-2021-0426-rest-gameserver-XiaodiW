using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubExplorer.Comm {

    public class RestApiGet : RestApiComm {
        public RestApiGet(Uri url) {
            Url = url;
        }
        public async Task<string> Get() {
            return await DoComm(Url,HttpMethod.Get, null);
        }
    }

}