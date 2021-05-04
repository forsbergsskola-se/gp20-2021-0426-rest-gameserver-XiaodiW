using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubExplorer.Comm {

    public class RestApiPost : RestApi {
        private readonly object _obj;
        public RestApiPost(Uri url, object obj) {
            Url = url;
            _obj = obj;
        }

        public async Task<string> Post() {
            return await DoComm(Url,HttpMethod.Get, _obj);
        }
    }

}